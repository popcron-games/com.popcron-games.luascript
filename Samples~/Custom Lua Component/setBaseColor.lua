#Exposed color=color

#OnEnable,Button
function assignColor()
	local renderer = transform.getComponent("MeshRenderer")
	if renderer ~= nil then
		local mpb = getMaterialPropertyBlock(renderer)
		local colorObj = colorFromHex(color)
		mpb.setColor("_BaseColor", colorObj)
		setMaterialPropertyBlock(renderer, mpb)
	end
end

#Update,EditorOnly
function checkIfChangedInEditor()
	if lastColor == nil then
		lastColor = ""
	end

	if color ~= lastColor then
		lastColor = color
		assignColor()
	end
end