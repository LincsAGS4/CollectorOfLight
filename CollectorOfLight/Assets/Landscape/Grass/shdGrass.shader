Shader "Saye/Grass"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BottomColour ("Bottom Colour", Color) = (1.0, 1.0, 1.0, 1.0)
		_TopColour ("Top Colour", Color) = (1.0, 1.0, 1.0, 0.0)
		_Height ("Height", float) = 0.0
		_Part ("Part (Layer / LayerCount)", float) = 0.0
	}
	SubShader
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		fixed4 _BottomColour;
		fixed4 _TopColour;
		float _Height;
		float _Part;
		
		float2 _PlayerX;
		float2 _PlayerZ;
		
		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
		};
		
		void vert(inout appdata_full v)
		{
			v.vertex.xyz += v.normal * _Height * _Part;
		}
		
		void surf (Input i, inout SurfaceOutput o)
		{
			float2 uv = i.uv_MainTex;
			uv.x += 0.05f * sin(i.worldPos.y + _Time.y * 0.5f) * _Part * _Part * _Part;
			uv.y += 0.05f * cos(i.worldPos.y + _Time.y * 0.5f) * _Part * _Part * _Part;
		
			fixed4 colour = tex2D(_MainTex, uv);

			// Check that the colour isn't transparent.
			if (colour.a != 0)
			{
				// Set the colour based on the part.
				colour.r = lerp(_BottomColour.r, _TopColour.r, _Part);
				colour.g = lerp(_BottomColour.g, _TopColour.g, _Part);
				colour.b = lerp(_BottomColour.b, _TopColour.b, _Part);
				colour.a = lerp(_BottomColour.a, _TopColour.a, _Part);
				
				// Add a random colouration.
				float random = sin(i.worldNormal.x * i.worldNormal.z + i.worldNormal.y * i.worldPos.y) + tan(i.worldPos.y + i.worldNormal.z + i.worldNormal.y + i.worldNormal.x);
				colour.r += 0.2 * abs(sin(0.05 * i.worldPos.y * (i.worldNormal.x + i.worldNormal.y + i.worldNormal.z)));
				
				o.Albedo = colour.rgb;
			}
			else
			clip(-1);
			
			o.Alpha = colour.a;
		}

		ENDCG
	}
	FallBack "Diffuse"
}

// Vertex/Fragment Shader Version (No Lighting) (Requires a float variable called Detail because tiling didn't work, I also didn't implement swaying in this version)
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//
//			uniform sampler2D _MainTex;
//			uniform fixed4 _BottomColour;
//			uniform fixed4 _TopColour;
//			uniform float _Detail;
//			uniform float _Height;
//			uniform float _Part;
//			
//			struct vInput
//			{
//				float4 vertex : POSITION;
//				float3 normal : NORMAL;
//				float4 texcoord0 : TEXCOORD0;
//			};
//			
//			struct fInput
//			{
//				float4 position : SV_POSITION;
//				float4 texcoord0 : TEXCOORD0;
//			};
//			
//			fInput vert(vInput i)
//			{
//				fInput o;
//				o.position = mul(UNITY_MATRIX_MVP, i.vertex);
//				o.position.xyz += i.normal * _Height * _Part;
//				o.texcoord0 = i.texcoord0 * _Detail;
//				return o;
//			}
//			
//			fixed4 frag(fInput i) : SV_TARGET
//			{
//				fixed4 colour = tex2D(_MainTex, i.texcoord0);
//				
//				// Fragment is transparent, ignore it.
//				if (colour.a == 0)
//					discard;
//					
//				colour.r = lerp(_BottomColour.r, _TopColour.r, _Part);
//				colour.g = lerp(_BottomColour.g, _TopColour.g, _Part);
//				colour.b = lerp(_BottomColour.b, _TopColour.b, _Part);
//				colour.a = lerp(_BottomColour.a, _TopColour.a, _Part);
//					
//				return colour;
//			}
//
//			ENDCG
