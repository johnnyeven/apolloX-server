WriteLine("Lua: ------------加载地图数据(id:0001)------------")
local mapData = {
	{id = "MONSTER_0001", amount = 5, x = 4275, y = 2950, range = 300},
	{id = "MONSTER_0002", amount = 10, x = 1500, y = 1500, range = 100}
}

for k, v in ipairs(mapData) do
	WriteLine("Lua: 开始创建角色 " .. v.id .. "...")
	createMonsterRange('0001', v.id, v.x, v.y, v.amount, v.range)
end
WriteLine("Lua: ----------加载地图数据(id:0001)完成----------")