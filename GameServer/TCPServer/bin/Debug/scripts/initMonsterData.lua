WriteLine("Lua: ---------------加载Monster数据--------------")
data = {
	["MONSTER_0001"] = {
		level = 1,
		name = "摄魂怪",
		resource = "char9",
		speed = 5,
		health = 1000,
		mana = 300,
		attackRange = 50,
		attackSpeed = 0.5,
		passitiveMonster = false
	},
	["MONSTER_0002"] = {
		level = 1,
		name = "蜘蛛精",
		resource = "char8",
		speed = 7,
		health = 1200,
		mana = 500,
		attackRange = 200,
		attackSpeed = 1,
		passitiveMonster = false
	}
}

for k in pairs(data) do
	WriteLine("加载 " .. data[k].name .. "...")
	registerMonsterData(k, data[k].name, data[k].level, data[k].resource, data[k].speed, data[k].health, data[k].mana, data[k].attackRange, data[k].attackSpeed, data[k].passitiveMonster)
end
WriteLine("Lua: ---------------加载Monster数据完成----------")