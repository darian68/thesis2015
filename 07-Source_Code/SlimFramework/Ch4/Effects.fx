float4 Vertex_Shader(float4 inPos : POSITION) : SV_POSITION
{
    return inPos;
}

float4 Pixel_Shader() : SV_TARGET
{
    return float4(0.0f, 0.0f, 1.0f, 1.0f);
}
