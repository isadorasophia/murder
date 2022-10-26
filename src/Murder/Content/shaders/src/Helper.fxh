//#define HALF_TEXEL_OFFSET

float is_same(float3 colorA, float3 colorB) {
    return !(abs(colorB.r - colorA.r) >= 0.01f 
          || abs(colorB.g - colorA.g) >= 0.01f
          || abs(colorB.b - colorA.b) >= 0.01f);
}

float angle(float2 from, float2 to) {
    return atan2(to.x - from.x, to.y - from.y);
}

float wrap_angle(float rad) {
    float deg = degrees(rad);
    return deg >= 0.0f ? deg : (360.0f + deg);
}

/*
float4 snap_pixel(float4 position, float2 renderTargetSize) {
    float hpcX = renderTargetSize.x * 0.5;
    float hpcY = renderTargetSize.y * 0.5;

#ifdef HALF_TEXEL_OFFSET
    float hpcOX = -0.5;
    float hpcOY = -0.5;
#else
    float hpcOX = 0;
    float hpcOY = 0;
#endif	

    float4 snappedPos = position;

    float pos = floor((position.x / position.w) * hpcX + 0.5f) + hpcOX;
    snappedPos.x = pos / hpcX * position.w;

    pos = floor((position.y / position.w) * hpcY + 0.5f) + hpcOY;
    snappedPos.y = pos / hpcY * position.w;

    return snappedPos;
    return position;
}

float4 samplePixel(sampler2D textureSampler, float2 textureCoord) {
    //return tex2Dgrad(textureSampler, frac(textureCoord), ddx(textureCoord), ddy(textureCoord));
    return tex2D(textureSampler, textureCoord);
}
*/

float3 to_HSL(float3 rgb) {
    float max_value = max(max(rgb.r, rgb.g), rgb.b);
    float min_value = min(min(rgb.r, rgb.g), rgb.b);

    float3 hsl;
    hsl.z = (max_value + min_value) / 2.0f;

    if (max_value == min_value) {
        hsl.x = hsl.y = 0.0f;
    } else {
        float d = max_value - min_value;
        hsl.y = hsl.z > .5f ? d / (2.0f - (max_value + min_value)) : d / (max_value + min_value);

        if (rgb.r > rgb.g && rgb.r > rgb.b) {
            hsl.x = (rgb.g - rgb.b) / d + (rgb.g < rgb.b ? 6.0f : 0.0f);
        } else if (rgb.g > rgb.b) {
            hsl.x = (rgb.b - rgb.r) / d + 2.0f;
        } else {
            hsl.x = (rgb.r - rgb.g) / d + 4.0f;
        }

        hsl.x = hsl.x / 6.0f;
    }

    return hsl;
}

float HUE_to_RGB(float p, float q, float t) {
    if (t < 0.0f) {
        t += 1.0f;
    }

    if (t > 1.0f) {
        t -= 1.0f;
    }

    if (t < 1.0f/6.0f) {
        return p + (q - p) * 6.0f * t;
    }

    if (t < 1.0f/2.0f) {
        return q;
    }

    if (t < 2.0f/3.0f) {
        return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
    }

    return p;
}

float3 from_HSL(float3 hsl) {
    float3 rgb;

    if (hsl.y == 0.0f) {
        rgb = float3(hsl.z, hsl.z, hsl.z);
    } else {
        float y = hsl.z < .5f ? hsl.z * (1.0f + hsl.y) : hsl.z + hsl.y - hsl.z * hsl.y;
        float x = 2.0f * hsl.z - y;
        rgb = float3(HUE_to_RGB(x, y, hsl.x + 1.0f / 3.0f), HUE_to_RGB(x, y, hsl.x), HUE_to_RGB(x, y, hsl.x - 1.0f / 3.0f));
    }

    return rgb;
}

bool is_at_outer_border(sampler2D textureSampler, float4 px, float2 textureCoord, float2 pxSize) {
    // current pixel should be transparent
    if (px.a > 0.01f) {
        return false;
    }

    // top pixel
    if (tex2D(textureSampler, textureCoord + float2(0.0f, -pxSize.y)).a > .01f) {
        return true;
    }

    // right pixel
    if (tex2D(textureSampler, textureCoord + float2(pxSize.x, 0.0f)).a > .01f) {
        return true;
    }

    // bottom pixel
    if (tex2D(textureSampler, textureCoord + float2(0.0f, pxSize.y)).a > .01f) {
        return true;
    }

    // left pixel
    if (tex2D(textureSampler, textureCoord + float2(-pxSize.x, 0.0f)).a > .01f) {
        return true;
    }

    return false;
}

bool is_at_inner_border(sampler2D textureSampler, float4 px, float2 textureCoord, float2 pxSize) {
    // current pixel should not be transparent
    if (px.a < 0.01f) {
        return false;
    }

    // top pixel
    if (tex2D(textureSampler, textureCoord + float2(0.0f, -pxSize.y)).a < .01f) {
        return true;
    }

    // right pixel
    if (tex2D(textureSampler, textureCoord + float2(pxSize.x, 0.0f)).a < .01f) {
        return true;
    }

    // bottom pixel
    if (tex2D(textureSampler, textureCoord + float2(0.0f, pxSize.y)).a < .01f) {
        return true;
    }

    // left pixel
    if (tex2D(textureSampler, textureCoord + float2(-pxSize.x, 0.0f)).a < .01f) {
        return true;
    }

    return false;
}