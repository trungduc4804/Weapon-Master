# WeaponMaster - Script Architecture

## Tong quan
Du an duoc to chuc theo cac module chinh trong `Assets/_Scripts`:
- `Dungeon`: tao map phong, dong/mo cua, kich hoat encounter.
- `Enemy`: logic enemy goc, AI state machine, cac bien the boss/skeleton.
- `Player`: di chuyen, tan cong, he thong vu khi melee/ranged.
- `Pathfinding`: A* custom (`Grid2D`, `Node`, `Pathfinding`, `Heap`).
- `Item`: vat pham roi ra va logic nhat item.
- `GameManager`: camera va UI.

## Cau truc thu muc script
```text
Assets/_Scripts
|- Dungeon
|  |- DungeonGenerator.cs
|  |- Room.cs
|  |- RoomTrigger.cs
|  |- EnemySpawner.cs
|  |- Door.cs
|- Enemy
|  |- EnemyBase.cs
|  |- EnemyAI.cs
|  |- EnemyBoss.cs
|  |- EnemySkeleton1.cs
|  |- EnemySkeleton2.cs
|  |- EnemySkeleton3.cs
|- Player
|  |- Player.cs
|  |- PlayerAttack.cs
|  |- WeaponBase.cs
|  |- MeleeWeapon.cs
|  |- RangedWeapon.cs
|  |- FirePoint.cs
|- Pathfinding
|  |- Grid2D.cs
|  |- Node.cs
|  |- Pathfinding.cs
|- Item
|  |- Item.cs
|- GameManager
   |- CameraManager.cs
   |- UIManagerImage.cs (class `UIManage`)
   |- UIManagerButton.cs (class `UIManagerBottom`)
```

## Quan he ke thua (Inheritance)
### Enemy
```text
EnemyBase (abstract)
  -> EnemyAI
      -> EnemyBoss
      -> EnemySkeleton1
      -> EnemySkeleton2
      -> EnemySkeleton3
```

### Weapon
```text
WeaponBase (abstract)
  -> MeleeWeapon
  -> RangedWeapon
```

## Vai tro tung script
### Dungeon
- `DungeonGenerator`: sinh dungeon theo grid, spawn start/normal/boss room, ket noi cua, dua player ve phong bat dau.
- `Room`: quan ly state encounter trong phong (`encounterStarted`, `enemiesAlive`, `cleared`), dong cua khi combat, mo cua khi clear.
- `RoomTrigger`: khi player vao trigger, goi `Room.PlayerEntered()` va cap nhat `Player.SetCurrentRoom()`.
- `EnemySpawner`: spawn enemy tai cac spawn point va gan `Room` cho tung enemy qua `EnemyBase.SetRoom()`.
- `Door`: dong/mo collider + visual cua cua.

### Enemy
- `EnemyBase`: stat co ban (HP/damage/speed), chase don gian, nhan damage, knockback, death, drop item, thong bao `Room.EnemyKilled()`.
- `EnemyAI`: state machine `Idle/Chase/Attack`, tim duong A* theo `Grid2D`, tan cong theo cooldown, sync animation.
- `EnemyBoss`, `EnemySkeleton1/2/3`: ke thua AI chung, override `TakeDamage/Die` de trigger animation hurt/die.

### Player + Weapon
- `Player`: di chuyen 2D, nhan damage, knockback, health/gold, luu `CurrentRoom`, goi camera di chuyen theo phong.
- `PlayerAttack`: quan ly vu khi hien tai, switch weapon (phim `1/2`), tan cong chuot trai, buff damage tu item.
- `WeaponBase`: base class truu tuong cho vu khi.
- `MeleeWeapon`: bat collider tam danh trong `attackDuration`, va cham enemy de gay damage.
- `RangedWeapon`: ban projectile ve huong chuot, truyen damage cho projectile.
- `FirePoint`: script tren projectile, xu ly va cham enemy va tu huy.

### Pathfinding
- `Grid2D`: tao grid node tu bounds phong, danh dau node walkable/block.
- `Node`: du lieu tung node + metadata cho A*.
- `Pathfinding`: trien khai A* custom, tra ve danh sach waypoint cho `EnemyAI`.
- `Heap<T>`: min-heap toi uu open-set trong A*.

### Item
- `ItemPickup` (`Item.cs`): item `Gold/Health/Damage`; va cham player se cong chi so va huy item.

### GameManager / UI
- `CameraManager`: singleton camera, `SnapToRoom` khi bat dau va `MoveToRoom` khi doi phong.
- `UIManage` (`UIManagerImage.cs`): cap nhat HP, coin, damage text/slider theo `Player` va `PlayerAttack`.
- `UIManagerBottom` (`UIManagerButton.cs`): xu ly button `Restart/Quit/Resume/Setting`, pause bang `Time.timeScale`.

## Quan he giua cac script (luong chinh)
### 1) Khoi tao dungeon
1. `DungeonGenerator.Start()` sinh cac phong.
2. `ConnectRooms()` bat/tat cua theo hang xom.
3. `MovePlayerToStart()` dat player vao start room, cap nhat camera.

### 2) Player vao phong
1. `RoomTrigger.OnTriggerEnter2D()` nhan dien player.
2. Goi `Room.PlayerEntered()` de bat encounter.
3. Goi `Player.SetCurrentRoom()` de camera di theo phong.

### 3) Spawn va combat
1. `Room` goi `EnemySpawner.SpawnEnemies(this)`.
2. Moi enemy duoc gan room qua `EnemyBase.SetRoom(room)`.
3. `EnemyAI.Update()` chi chase/attack khi player con song va dang trong cung room.
4. Neu enemy trong room con song -> `Room.CloseDoors()`.

### 4) Ket thuc encounter
1. Enemy chet -> `EnemyBase.Die()` -> `room.EnemyKilled()`.
2. Khi `enemiesAlive == 0`, `Room` danh dau `cleared` va mo cua.
3. Enemy co the roi item (`ItemPickup`) de buff cho player.

## Luu y thiet ke hien tai
- Naming chua dong bo giua ten file va ten class:
  - `UIManagerImage.cs` chua class `UIManage`.
  - `UIManagerButton.cs` chua class `UIManagerBottom`.
- `EnemyAI` co static cache player (`cachedPlayer`) de giam tim kiem `FindFirstObjectByType`.
- `EnemyAI` fallback chase truc tiep neu khong lay duoc `Grid2D`.
