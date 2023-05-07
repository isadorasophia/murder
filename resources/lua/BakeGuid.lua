filename = app.params["filename"]
output = app.params["output"]
guid = app.params["guid"]
layerIndex = tonumber(app.params["layer"])
sliceIndex = tonumber(app.params["slice"])

data = "guid:" .. guid

spr = app.open(filename)

if layerIndex < 0 then -- This sprite is NOT split by layers
	
	if #spr.slices == 0 then
	-- This is a single simple sprite, we should set the global userdata
		spr.data = data
		print("Global userdata was set for " .. output)
	else
	-- This is a sliced sprite, with no layers. Let's set the lace userdata
		spr.slices[sliceIndex + 1].data = data
		print("Slice ".. sliceIndex .. " userdata was set for " .. output)
	end

else -- This sprite is split by layers
	if #spr.slices <= 1 then
		spr.layers[layerIndex+1].data = data
	else
		print("<WARNING> " .. filename)
		print("Baking LAYER + SLICE data is not possible right now.")
	end
end

spr:saveCopyAs(output)
print("saved " .. output)
