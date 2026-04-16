# WeaponMaster - Script Architecture

## Tổng quan
Dự án được tổ chức theo các bộ module chuyên biệt bên trong `Assets/_Scripts`:
- `Dungeon`: Tự động tạo bản đồ phòng ngẫu nhiên, đóng/mở cửa, kích hoạt trận đấu với quái vật, hiển thị sương mù (fog-of-war) trên Minimap và tạo phòng Cửa hàng (Shop).
- `Enemy`: Logic điều khiển AI quái vật, hệ thống máy trạng thái (state machine), và các biến thể quái vật (Skeleton) / Boss.
- `Player`: Điều khiển di chuyển, nhận sát thương, hệ thống vũ khí cận chiến và tấn công tầm xa.
- `Pathfinding`: Thuật toán tìm đường A* tự code tay hoàn toàn (`Grid2D`, `Node`, `Pathfinding`, `Heap`).
- `Item`: Vật phẩm rớt ra trên sàn, hệ thống nhặt đồ và thanh thao tác nhanh vật phẩm (`QuickItemBar`).
- `Shop`: Hệ thống quản lý cửa hàng mua bán, UI cửa hàng và Trigger tương tác vật lý.
- `Audio`: Quản lý tổng thể hiệu ứng âm thanh, nhạc nền, cài đặt âm lượng bằng UI.
- `GameManager`: Quản lý Camera chính, Minimap Camera và các giao diện người dùng (UI) HUD.

## Cấu trúc thư mục Script mới nhất
```text
Assets/_Scripts
|- Dungeon
|  |- DungeonGenerator.cs (Kết hợp sinh phòng Boss & Shop)
|  |- Room.cs (Tích hợp logic sương mù Minimap)
|  |- RoomTrigger.cs
|  |- EnemySpawner.cs
|  |- Door.cs & BossDoorLock.cs
|- Enemy
|  |- EnemyBase.cs
|  |- EnemyAI.cs
|  |- EnemyBoss.cs
|  |- EnemySkeleton1/2/3.cs
|- Player
|  |- Player.cs
|  |- PlayerAttack.cs
|  |- WeaponBase.cs (Base class)
|  |- MeleeWeapon.cs & RangedWeapon.cs & FirePoint.cs
|- Pathfinding
|  |- Grid2D.cs & Node.cs & Pathfinding.cs
|- Item
|  |- Item.cs
|  |- QuickItemBar.cs (Quản lý túi phụ)
|  |- QuickItemSlotUI.cs
|- Shop [MỚI]
|  |- ShopManager.cs (Kho quản lý mua bán)
|  |- ShopInteractable.cs (Điểm click chuột Trigger mở shop)
|  |- ShopItemData.cs & ShopItemEffectApplier.cs
|  |- ShopItemEntryUI.cs
|- Audio [MỚI]
|  |- AudioManager.cs
|  |- AudioCueLibrary.cs
|  |- AudioSettingsPanel.cs & UIButtonSound.cs
|- GameManager
   |- CameraManager.cs
   |- MinimapCamera.cs [MỚI] (Cam phụ chiếu bản đồ nhỏ)
   |- UIManagerImage.cs (class `UIManage`)
   |- UIManagerButton.cs (class `UIManagerBottom`)
   |- UIButtonMenu.cs
```

## Quan hệ kế thừa (Inheritance)
### Hướng tiếp cận cho Quái (Enemy)
```text
EnemyBase (Lớp Abstract)
  -> EnemyAI (Logic tìm đường & Đánh)
      -> EnemyBoss
      -> EnemySkeleton1 / 2 / 3
```

### Hướng tiếp cận cho Vũ khí (Weapon)
```text
WeaponBase (Lớp Abstract)
  -> MeleeWeapon (Vũ khí xáp lá cà)
  -> RangedWeapon (Vũ khí đánh xa)
```

## Vai trò chi tiết của từng Script quan trọng
### Dungeon (Hầm ngục)
- `DungeonGenerator`: Sinh dungeon theo lưới ô vuông (grid). Rải phòng Start, phân bố các phòng thường, phòng Boss và phòng Shop riêng biệt, sau đó nối Collider của các Cửa (Door) lại với nhau.
- `Room`: Quản lý trạng thái vòng chiến đấu. Đóng cửa khi có quái, mở cửa khi diệt xong toàn bộ quái vật nhánh. Tính năng Minimap tích hợp: giấu Icon của phòng trên Minimap cho đến khi người chơi bước qua khe cửa.
- `RoomTrigger`: Trigger dùng để kích hoạt `Room.PlayerEntered()`.

### Shop & Item (Cửa hàng & Đồ vật)
- `ShopManager`: Xử lý giao dịch. Trừ tiền `Player.gold`, chặn nếu không đủ tiền hoặc mua quá giới hạn. Đẩy vật phẩm mua thành công vào thanh `QuickItemBar`.
- `ShopInteractable`: Trigger nhận tương tác chuột. Tính toán khoảng cách vật lý của người chơi để tránh tình trạng "mua đồ từ xa".
- `QuickItemBar`: Thanh hotbar ở dưới màn hình để chứa các vật phẩm hồi phục dạng tiêu hao.
- `Item`: Tiền vàng hoặc máu bị rớt ra từ quái (có script nam châm hút vào người chơi).

### Hệ thống Camera & Âm thanh
- `MinimapCamera`: Nhìn từ trên cao xuống và render texture ra góc màn hình. Có tính năng bám đuổi mềm mại (Lerp) theo người chơi và tự động điều chỉnh ống kính to nhỏ.
- `AudioManager`: Script Singleton lo mọi tín hiệu âm nhạc, từ mở khóa cửa, lụm tiền đến nhạc nền đổi tuỳ hứng. 
- `CameraManager`: Đẩy camera nhảy (Snap) từng nấc theo từng ô phòng của Dungeon lúc di chuyển.

## Luồng hoạt động chính (Flow)
### 1. Khởi tạo đầu game
1. `DungeonGenerator.Start()` tạo chuỗi vòng lặp rải các căn phòng.
2. Quét vị trí tạo thêm `ShopRoom` và kết nối nó vào luồng cửa như bình thường.
3. Chỉnh tọa độ `CameraManager` và `MinimapCamera` đè chuẩn vào tâm Start Room để người chơi không bị lag góc nhìn.

### 2. Giao tiếp khi Mua bán Đồ
1. Khi tiếp cận thương nhân và click chuột, `ShopInteractable.OnMouseDown()` đo khoảng cách tới Player.
2. Nếu đủ gần (< 5 Unit), gọi chéo sang `ShopManager.OpenShop()` làm đóng băng thời gian `Time.timeScale = 0`.
3. Player click mua -> Mất vàng -> Nhận Effect hoặc được đẩy vật phẩm vào `QuickItemBar` để ấn phím dùng dần.

### 3. Vòng lặp Hệ thống Bản đồ chìm (Minimap)
1. Layer `Minimap` ban đầu bị Room khóa lại. Màn hình radar đen thui.
2. Khi player đi xuyên cửa vào phòng mới, `RoomTrigger` bắt tín hiệu gửi chéo bật sáng Icon Map của phòng đó lên. Layer Minimap cập nhật hiển thị.
3. Bản thân `MinimapCamera` lướt di chuyển song song ở bên trên cao bám theo tọa độ nhân vật chuẩn xác 1:1.

## Ghi chú thiết kế hiện tại & Kỹ thuật cần lưu ý
- Hiện trạng lỗi đặt tên còn xuất hiện:
  - Text code ở file `UIManagerImage.cs` lại chứa class tên `UIManage`.
  - Phím điều khiển `UIManagerButton.cs` chứa class tên `UIManagerBottom`.
- Thuật toán A* hoàn toàn chạy tự đóng gói nội bộ không dùng NavMesh của Unity. `EnemyAI` lưu trữ tĩnh `cachedPlayer` để tránh hàm FindObjectByType lặp đi lặp lại. Giúp map rộng lên tới hàng trăm con quái không bị tụt FPS.
