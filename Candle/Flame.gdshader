shader_type spatial;
render_mode unshaded, shadows_disabled, specular_disabled, ambient_light_disabled, cull_disabled;

void fragment() {
	ALBEDO = COLOR.rgb;
}