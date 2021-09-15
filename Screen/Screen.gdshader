shader_type canvas_item;

uniform int kFragSize = 12;

void fragment() {
    vec2 pixelSize = TEXTURE_PIXEL_SIZE * float(kFragSize);
    vec2 pixel = pixelSize * floor(SCREEN_UV / pixelSize);
    COLOR.rgb = texture(TEXTURE, pixel).rgb;
}
