using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using Microsoft.DirectX.Direct3D;
using Framework.Physics;
using System.Collections;
using System.IO;

namespace CastleButcher.GameEngine.Resources
{
    /// <summary>
    /// £aduje i przechowuje podstawowe zasoby takie jak meshe textury i shadery pilnuj¹c ¿eby ka¿dy by³ wczytany tylko raz
    /// </summary>

    class MeshEntry
    {
        public Mesh Mesh;
        public MaterialData[] Materials;
        public Material[] DxMaterials;
    }
    class RenderingDataEntry
    {
        public RenderingData RData;
    }
    class DxTextureEntry
    {
        public Texture Texture;
    }
    class TextureEntry
    {
        public IMapData Texture;
    }
    class DxEffectEntry
    {
        public Effect Effect;
    }
    class MaterialEntry
    {
        public MaterialData Material;
    }

    class EffectEntry
    {
        public EffectData Effect;
    }
    class AnimatedMapEntry
    {
        public AnimatedMapData MapData;
    }


    
    public class ResourceCache:IDeviceRelated
    {
        public static ResourceCache Instance
        {
            get
            {
                return Singleton<ResourceCache>.Instance;
            }
        }
        Hashtable meshes;
        Hashtable renderingDatas;
        Hashtable dxTextures;
        Hashtable textures;
        Hashtable dxEffects;
        Hashtable effects;
        Hashtable collisionMeshes;
        Hashtable materials;
        Hashtable animatedTextures;

        Device device;

        private ResourceCache()
        {
            meshes = new Hashtable();
            renderingDatas = new Hashtable();
            collisionMeshes = new Hashtable();
            dxEffects = new Hashtable();
            textures = new Hashtable();
            dxTextures = new Hashtable();
            materials = new Hashtable();
            effects = new Hashtable();
            animatedTextures = new Hashtable();
        }

        //public void AddResource(string fileName)
        //{
        //    int i=fileName.LastIndexOf('.');
        //    string ext = fileName.Substring(i+1, fileName.Length - i - 1);
        //    switch (ext)
        //    {
        //        case "wd":

        //            break;
        //    }
        //}
        private MeshEntry LoadMesh(string fileName)
        {
            if (device == null)
                throw new Exception("DeviceNotInitialized!!");

            ExtendedMaterial[] materials;
            MeshEntry entry = new MeshEntry();
            Mesh mesh = Mesh.FromFile(AppConfig.MeshPath + fileName, MeshFlags.Managed, device, out materials);
            
            int[] adjacency = new int[3 * mesh.NumberFaces];
            mesh.GenerateAdjacency(0, adjacency);
            mesh = mesh.Optimize(MeshFlags.OptimizeCompact | MeshFlags.OptimizeStripeReorder | MeshFlags.OptimizeAttributeSort, adjacency);
            entry.Mesh = mesh.Clone(mesh.Options.Value, Framework.Vertex.PositionNTBTextured.Declarator, device);
            
            mesh.Dispose();
            //if (fileName != "sphere.x") 
            
            entry.Mesh.ComputeTangent(0, 0, 0, 0);

            entry.Materials = new MaterialData[materials.Length];
            entry.DxMaterials = new Material[materials.Length];

            for(int i=0;i<materials.Length;i++)
            {
                MaterialData data = null;
                entry.DxMaterials[i] = materials[i].Material3D;
                entry.DxMaterials[i].Ambient = entry.DxMaterials[i].Diffuse;
                
                string temp = materials[i].TextureFilename;
                if (temp != null)
                {
                    materials[i].TextureFilename = temp.Substring(temp.LastIndexOf('\\') + 1);

                    temp = materials[i].TextureFilename.Substring(0,
                        materials[i].TextureFilename.LastIndexOf('.'));
                    if (temp.EndsWith("_diff"))
                        temp = temp.Substring(0, temp.Length - 5);
                    temp += ".mat";

                    data = GetMaterialData(temp);
                    //if (data == null)
                        //throw new Exception("Nie uda³o siê za³adowaæ materia³u " + temp + " w meshu " + fileName);
                }
                else
                {
                    data = null; 
                }
                entry.Materials[i] = data;
                
            }
            

            return entry;
        }
        private RenderingDataEntry LoadRenderingData(string fileName)
        {
            //if (device == null)
            //    throw new Exception("DeviceNotInitialized!!");

            RenderingDataEntry result = new RenderingDataEntry();
            result.RData = RenderingData.FromFile(AppConfig.MeshPath + fileName);
            result.RData.CustomTransform = Microsoft.DirectX.Matrix.Identity;
            return result;
        }
        private CollisionMesh LoadCollisionMesh(string fileName)
        {
            //if (device == null)
            //    throw new Exception("DeviceNotInitialized!!");

            CollisionMesh result = CollisionMesh.FromFile(AppConfig.MeshPath + fileName, false);
            return result;
        }
        private DxTextureEntry LoadDxTexture(string fileName)
        {
            if (device == null)
                throw new Exception("DeviceNotInitialized!!");

            DxTextureEntry result=new DxTextureEntry();
            result.Texture = TextureLoader.FromFile(device, AppConfig.TexturePath + fileName);
            //result.FileName = fileName;
            return result;
        }
        private TextureEntry LoadTexture(string fileName)
        {
            if (device == null)
                throw new Exception("DeviceNotInitialized!!");

            TextureEntry result = new TextureEntry();
            if (fileName.Substring(fileName.Length - 4) == ".mat")
            {
                result.Texture = MapData.FromFile(AppConfig.TexturePath + fileName);
                //result.FileName = fileName;
            }
            else if (fileName.Substring(fileName.Length - 4) == ".amd")
            {
                AnimatedMapData amd = GetAnimatedTexture(fileName);
                MapAnimation anim = amd.GetAnimationInstance();
                result.Texture = anim;
                anim.Start();
                anim.Loop = true;
                GM.AppWindow.AddUpdateableItem(anim);
                //result.FileName = fileName;
            }
            else
            {
                result.Texture = new MapData(GetDxTexture(fileName), fileName);
            }
            return result;
            
        }
        private DxEffectEntry LoadDxEffect(string fileName)
        {
            if (device == null)
                throw new Exception("DeviceNotInitialized!!");

            DxEffectEntry result=new DxEffectEntry();
            string errors;
            //result.Effect = Effect.FromFile(device,AppConfig.EffectPath + fileName, null, ShaderFlags.Debug|ShaderFlags.SkipOptimization, null,out errors);
            result.Effect = Effect.FromFile(device, AppConfig.EffectPath + fileName, null, null,
                ShaderFlags.Debug | ShaderFlags.SkipOptimization, null, out errors);
            if (result.Effect == null)
                throw new Exception(errors);
            //result.FileName = fileName;
            return result;
        }
        private EffectEntry LoadEffect(string fileName)
        {
            if (device == null)
                throw new Exception("DeviceNotInitialized!!");

            EffectEntry result = new EffectEntry();
            result.Effect = EffectData.FromFile(fileName);
            //result.FileName = fileName;
            return result;
        }

        private MaterialEntry LoadMaterial(string fileName)
        {
            MaterialEntry result = new MaterialEntry();
            try
            {
                result.Material = MaterialData.FromFile(AppConfig.TexturePath + fileName);
            }
            catch
            {
                result.Material = null;
            }
            return result;
        }

        private AnimatedMapEntry LoadAnimatedTexture(string fileName)
        {
            AnimatedMapEntry result = new AnimatedMapEntry();
            result.MapData = AnimatedMapData.FromFile(AppConfig.TexturePath + fileName);
            return result;
        }
        
        /// <summary>
        /// Zwraca obiekt mesh, jeœli nie istnieje jeszcze to próbuje wczytaæ
        /// </summary>
        /// <param name="fileName">Nazwa pliku</param>
        /// <returns>Obiekt Mesh</returns>
        public Mesh GetMesh(string fileName,out MaterialData[] materials,out Material[] dxMaterials)
        {
            MeshEntry result = (MeshEntry)meshes[fileName];

            if (result == null)
            {
                result = LoadMesh(fileName);
                meshes[fileName] = result;

                materials = result.Materials;
                dxMaterials = result.DxMaterials;
                return result.Mesh;
            }
            else
            {
                materials = result.Materials;
                dxMaterials = result.DxMaterials;
                return result.Mesh;
            }
        }

        /// <summary>
        /// Zwraca obiekt RenderingData, jeœli nie istnieje jeszcze to próbuje wczytaæ
        /// </summary>
        /// <param name="fileName">Nazwa pliku</param>
        /// <returns>Obiekt CollisionMesh</returns>
        public RenderingData GetRenderingData(string fileName)
        {
            RenderingDataEntry result = (RenderingDataEntry)renderingDatas[fileName];
            if (result == null)
            {
                result = LoadRenderingData(fileName);
                renderingDatas[fileName] = result;
                return result.RData;
            }
            else
                return result.RData;
        }

        /// <summary>
        /// Zwraca obiekt CollisionMesh, jeœli nie istnieje jeszcze to próbuje wczytaæ
        /// </summary>
        /// <param name="fileName">Nazwa pliku</param>
        /// <returns>Obiekt CollisionMesh</returns>
        public CollisionMesh GetCollisionMesh(string fileName)
        {
            CollisionMesh result = (CollisionMesh)collisionMeshes[fileName];
            if (result == null)
            {
                result = LoadCollisionMesh(fileName);
                collisionMeshes[fileName] = result;
                return result;
            }
            else
                return result;
        }

        /// <summary>
        /// Zwraca obiekt Texture, jeœli nie istnieje jeszcze to próbuje wczytaæ
        /// </summary>
        /// <param name="fileName">Nazwa pliku</param>
        /// <returns>Obiekt Texture</returns>
        public Texture GetDxTexture(string fileName)
        {
            DxTextureEntry result = (DxTextureEntry)dxTextures[fileName];
            if (result == null)
            {
                result=LoadDxTexture(fileName);
                dxTextures[fileName]=result;
                return result.Texture;
            }
            else
                return result.Texture;
        }

        public IMapData GetTexture(string fileName)
        {
            TextureEntry result = (TextureEntry)textures[fileName];
            if (result == null)
            {
                result = LoadTexture(fileName);
                textures[fileName] = result;
                return result.Texture;
            }
            else
                return result.Texture;
        }

        /// <summary>
        /// Zwraca obiekt Effect, jeœli nie istnieje jeszcze to próbuje wczytaæ
        /// </summary>
        /// <param name="fileName">Nazwa pliku</param>
        /// <returns>Obiekt Effect</returns>
        public Effect GetDxEffect(string fileName)
        {
            DxEffectEntry result = (DxEffectEntry)dxEffects[fileName];
            if (result == null)
            {
                result = LoadDxEffect(fileName);
                dxEffects[fileName] = result;
                return result.Effect;
            }
            else
                return result.Effect;
        }

        /// <summary>
        /// Zwraca obiekt Effect, jeœli nie istnieje jeszcze to próbuje wczytaæ
        /// </summary>
        /// <param name="fileName">Nazwa pliku</param>
        /// <returns>Obiekt Effect</returns>
        public EffectData GetEffect(string fileName)
        {
            EffectEntry result = (EffectEntry)effects[fileName];
            if (result == null)
            {
                result = LoadEffect(fileName);
                effects[fileName] = result;
                return result.Effect;
            }
            else
                return result.Effect;
        }

        public MaterialData GetMaterialData(string fileName)
        {
            MaterialEntry result = (MaterialEntry)materials[fileName];
            if (result == null)
            {
                result = LoadMaterial(fileName);
                materials[fileName] = result;
                return result.Material;
            }
            else
                return result.Material;
        }
        public AnimatedMapData GetAnimatedTexture(string fileName)
        {
            AnimatedMapEntry result = (AnimatedMapEntry)animatedTextures[fileName];
            if (result == null)
            {
                result = LoadAnimatedTexture(fileName);
                animatedTextures[fileName] = result;
                return result.MapData;
            }
            else
                return result.MapData;
        }



        #region IDeviceRelated Members

        public void OnCreateDevice(Device device)
        {
            this.device = device;
            //IDictionaryEnumerator en = meshes.GetEnumerator();
            //while (en.MoveNext())
            //{
            //    DictionaryEntry entry = (DictionaryEntry)en.Current;
            //}
            //for (int i = 0; i < meshes.Count; i++)
            //{
            //    meshes.Values[i] = LoadMesh((string)meshes.Keys[i]);
            //}
            //foreach (DictionaryEntry entry in meshes)
            //{
            //    meshes[entry.Key] = LoadMesh((string)entry.Key);
            //}
            //throw new Exception("The method or operation is not implemented.");
        }

        public void OnResetDevice(Device device)
        {

            //foreach (DictionaryEntry entry in dxTextures)
            //{
            //    DxTextureEntry m = LoadDxTexture((string)entry.Key);
            //    ((DxTextureEntry)entry.Value).Texture = m.Texture;
            //}
            //foreach (DictionaryEntry entry in meshes)
            //{
            //    MeshEntry m = LoadMesh((string)entry.Key);
            //    ((MeshEntry)entry.Value).Mesh = m.Mesh;
            //    ((MeshEntry)entry.Value).DxMaterials = m.DxMaterials;
            //    ((MeshEntry)entry.Value).Materials = m.Materials;
            //}

            //foreach (DictionaryEntry entry in materials)
            //{
            //    MaterialEntry m = LoadMaterial((string)entry.Key);
            //    ((MaterialEntry)entry.Value).Material = m.Material;
            //}

            foreach (DictionaryEntry entry in dxEffects)
            {
                DxEffectEntry m = LoadDxEffect((string)entry.Key);
                ((DxEffectEntry)entry.Value).Effect = m.Effect;
            }

            foreach (DictionaryEntry entry in effects)
            {
                ((EffectEntry)entry.Value).Effect.Reload();
            }


            
            //for(int i=0;i<meshes.Count;i++)
            //{
            //    meshes.Values[i] = LoadMesh((string)meshes.Keys[i]);
            //    meshes.
            //    //meshes[entry.Key] = LoadMesh((string)entry.Key);                
            //}
            //List<string> keys;
            //foreach(string

            //foreach (DictionaryEntry entry in meshes)
            //{
            //    Texture tex = meshes[entry.Key];
            //    //meshes[entry.Key] = LoadMesh((string)entry.Key);
            //    if(tex.
            //}
            //foreach (DictionaryEntry entry in renderingDatas)
            //{
            //    ((RenderingDataEntry)entry.Value).RData.Reload();
            //}
        }

        public void OnLostDevice(Device device)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public void OnDestroyDevice(Device device)
        {
            device = null;
            foreach (DictionaryEntry entry in meshes)
            {
                ((MeshEntry)entry.Value).Mesh.Dispose();
            }
            foreach (DictionaryEntry entry in dxEffects)
            {
                ((DxEffectEntry)entry.Value).Effect.Dispose();
            }
            foreach (DictionaryEntry entry in dxTextures)
            {
                ((DxTextureEntry)entry.Value).Texture.Dispose();
            }
        }

        #endregion

        
    }
}

