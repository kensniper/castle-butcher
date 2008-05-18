using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Framework.MyMath;
using CastleButcher.GameEngine.Resources;

namespace CastleButcher.GameEngine.Graphics
{
    public class ConstantsMap
    {
        Vector4 cameraPosition = Vector4.Empty;

        public Vector4 CameraPosition
        {
            get { return cameraPosition; }
            set { cameraPosition = value; }
        }
        Vector4 cameraUp = Vector4.Empty;

        public Vector4 CameraUp
        {
            get { return cameraUp; }
            set { cameraUp = value; }
        }
        Vector4 cameraLook = Vector4.Empty;

        public Vector4 CameraLook
        {
            get { return cameraLook; }
            set { cameraLook = value; }
        }


        Vector4 cameraVelocity = Vector4.Empty;

        public Vector4 CameraVelocity
        {
            get { return cameraVelocity; }
            set { cameraVelocity = value; }
        }
        Vector4 cameraAcceleration = Vector4.Empty;

        public Vector4 CameraAcceleration
        {
            get { return cameraAcceleration; }
            set { cameraAcceleration = value; }
        }

        Matrix world = Matrix.Identity;

        public Matrix World
        {
            get { return world; }
            set
            {
                world = value;
                worldI = Matrix.Invert(world);
                worldIT = Matrix.TransposeMatrix(worldI);
                worldView = world * View;
                worldViewProjection = world * view * projection;
            }
        }
        Matrix view = Matrix.Identity;

        public Matrix View
        {
            get { return view; }
            set
            {
                view = value;

                worldView = world * View;
                worldViewProjection = world * view * projection;
            }
        }
        Matrix projection = Matrix.Identity;

        public Matrix Projection
        {
            get { return projection; }
            set 
            {
                projection = value;

                worldViewProjection = world * view * projection;
            }
        }

        Matrix worldView = Matrix.Identity;

        public Matrix WorldView
        {
            get { return worldView; }
        }
        Matrix worldViewProjection = Matrix.Identity;

        public Matrix WorldViewProjection
        {
            get { return worldViewProjection; }
        }
        Matrix worldIT = Matrix.Identity;

        public Matrix WorldIT
        {
            get
            { 
                return worldIT;
            }
        }

        Matrix worldI = Matrix.Identity;
        public Matrix WorldI
        {
            get
            {
                return worldI;
            }
        }

        Vector4 lightDirection;

        public Vector4 LightDirection
        {
            get { return lightDirection; }
            set { lightDirection = value; }
        }

        ColorValue lightColor;

        public ColorValue LightColor
        {
            get { return lightColor; }
            set { lightColor = value; }
        }


        public void SetCamera(MyVector pos, MyVector up, MyVector look)
        {
            cameraPosition.X = pos.X;
            cameraPosition.Y = pos.Y;
            cameraPosition.Z = pos.Z;

            cameraUp.X = up.X;
            cameraUp.Y = up.Y;
            cameraUp.Z = up.Z;

            cameraLook.X = look.X;
            cameraLook.Y = look.Y;
            cameraLook.Z = look.Z;
        }

        public void SetCamera(MyVector pos, MyVector up, MyVector look,MyVector vel)
        {
            cameraPosition.X = pos.X;
            cameraPosition.Y = pos.Y;
            cameraPosition.Z = pos.Z;

            cameraUp.X = up.X;
            cameraUp.Y = up.Y;
            cameraUp.Z = up.Z;

            cameraLook.X = look.X;
            cameraLook.Y = look.Y;
            cameraLook.Z = look.Z;

            cameraVelocity.X = vel.X;
            cameraVelocity.Y = vel.Y;
            cameraVelocity.Z = vel.Z;
        }

        public void SetMatrices(Matrix world, Matrix view)
        {
            this.world = world;
            this.view = view;

            worldIT = Matrix.TransposeMatrix(Matrix.Invert(world));
            worldView = world * View;
            worldViewProjection = world * view * projection;
        }

        public void SetMatrices(Matrix world, Matrix view, Matrix proj)
        {
            this.world = world;
            this.view = view;
            this.projection = proj;

            worldI = Matrix.Invert(world);
            worldIT = Matrix.TransposeMatrix(worldI);
            worldView = world * View;
            worldViewProjection = world * view * proj;
        }

        public EffectHandle[] GetParameters(Effect effect)
        {
            EffectHandle[] handles = new EffectHandle[14];
            handles[0] = effect.GetParameter(null, "CameraPosition");
            handles[1] = effect.GetParameter(null, "CameraUp");
            handles[2] = effect.GetParameter(null, "CameraLook");
            handles[3] = effect.GetParameter(null, "CameraVelocity");
            handles[4] = effect.GetParameter(null, "CameraAcceleration");

            handles[5] = effect.GetParameter(null, "World");
            handles[6] = effect.GetParameter(null, "View");
            handles[7] = effect.GetParameter(null, "Projection");
            handles[8] = effect.GetParameter(null, "WorldView");
            handles[9] = effect.GetParameter(null, "WorldViewProjection");
            handles[10] = effect.GetParameter(null, "WorldI");
            handles[11] = effect.GetParameter(null, "WorldIT");

            handles[12] = effect.GetParameter(null, "LightDirection");
            handles[13] = effect.GetParameter(null, "LightColor");

            return handles;

        }

        public void SetEffectParameters(Effect effect,EffectHandle[] handles)
        {
            if(handles[0]!=null)
                effect.SetValue("CameraPosition", CameraPosition);
            if (handles[1] != null)
                effect.SetValue("CameraUp",CameraUp);
            if (handles[2] != null)
                effect.SetValue("CameraLook", CameraLook);
            if (handles[3] != null)
                effect.SetValue("CameraVelocity", CameraVelocity);
            if (handles[4] != null)
                effect.SetValue("CameraAcceleration", CameraAcceleration);

            if (handles[5] != null)
                effect.SetValue("World", World);
            if (handles[6] != null)
                effect.SetValue("View", View);
            if (handles[7] != null)
                effect.SetValue("Projection", Projection);
            if (handles[8] != null)
                effect.SetValue("WorldView", WorldView);
            if (handles[9] != null)
                effect.SetValue("WorldViewProjection", WorldViewProjection);
            if (handles[10] != null)
                effect.SetValue("WorldI", WorldI);
            if (handles[11] != null)
                effect.SetValue("WorldIT", WorldIT);

            if (handles[12] != null)
                effect.SetValue("LightDirection", LightDirection);
            if (handles[13] != null)
                effect.SetValue("LightColor", LightColor);            

        }
        public void SetParameters(EffectData effect)
        {
            SetEffectParameters(effect.DxEffect, effect.ParamHandles);
        }


    }
}
