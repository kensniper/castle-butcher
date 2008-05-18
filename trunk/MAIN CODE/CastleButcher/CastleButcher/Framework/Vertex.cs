using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework.Vertex
{
    struct PositionNTBTextured
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 Binormal;
        public Vector2 Coords;
        public static readonly VertexElement[] Declarator = new VertexElement[]
        {
            new VertexElement( 0, 0, DeclarationType.Float3,
            DeclarationMethod.Default, DeclarationUsage.Position, 0 ),
            new VertexElement( 0, 12, DeclarationType.Float3,
            DeclarationMethod.Default, DeclarationUsage.Normal, 0 ),
            new VertexElement( 0, 24, DeclarationType.Float3,
            DeclarationMethod.Default, DeclarationUsage.Tangent, 0 ),
            new VertexElement( 0, 36, DeclarationType.Float3,
            DeclarationMethod.Default, DeclarationUsage.BiNormal, 0 ),
            new VertexElement( 0, 48, DeclarationType.Float2,
            DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0 ),
            VertexElement.VertexDeclarationEnd
        };
        
    }
}