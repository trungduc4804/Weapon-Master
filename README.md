# Weapon Master - 2D Roguelike Action Game

Weapon Master là một trò chơi hành động Roguelike 2D được xây dựng bằng Unity Engine. Trò chơi tập trung vào cơ chế chiến đấu linh hoạt, hệ thống hầm ngục ngẫu nhiên và các cơ chế quản lý dữ liệu tối ưu, mang lại trải nghiệm thử thách và lôi cuốn cho người chơi.

## 🚀 Tính năng chính (Core Features)

- **Chiến đấu đa dạng:** Hệ thống vũ khí cận chiến (Melee) và tầm xa (Ranged) với cơ chế nhắm mục tiêu (Aiming) mượt mà.
- **Hầm ngục ngẫu nhiên (Procedural Generation):** Các phòng (Rooms) được tạo và kết nối ngẫu nhiên mỗi lần chơi, bao gồm phòng quái vật, phòng shop, phòng Gacha và phòng Boss.
- **Hệ thống Shop & Gacha:** Cho phép người chơi mua sắm vật phẩm và thử vận may để nhận được trang bị mạnh hơn.
- **Giải đố (Puzzle System):** Tích hợp các câu đố mini-game để đa dạng hóa gameplay.
- **Hệ thống Lưu trữ (Persistence):** Lưu lại Vàng, kỷ lục (Highscore) và tiến trình chơi thông qua hệ thống JSON Serialization.
- **Cảm giác chơi (Game Feel):** Hiệu ứng rung màn hình (Screen Shake), Hitstop và âm thanh phản hồi sống động.

## 🛠️ Điểm nhấn Kỹ thuật (Technical Highlights)

Dự án được xây dựng với tư duy lập trình bền vững và tối ưu hóa hiệu suất:

### 1. Tối ưu hóa với Object Pooling
Triển khai hệ thống **Generic Object Pooling** cho đạn, hiệu ứng hạt và kẻ địch. Điều này giúp giảm thiểu việc `Instantiate/Destroy` liên tục, ngăn chặn tình trạng giật lag do Garbage Collection (GC) gây ra.

### 2. Thiết kế hướng dữ liệu (Data-driven Design)
Sử dụng **ScriptableObjects** để quản lý thông số kẻ địch, vũ khí và vật phẩm. Giúp việc cân bằng game (Balancing) trở nên dễ dàng hơn mà không cần can thiệp sâu vào mã nguồn.

### 3. Kiến trúc Singleton & Quản lý tập trung
Áp dụng **Singleton Pattern** cho các Manager cốt lõi như:
- `AudioManager`: Quản lý tập trung âm thanh 2D/3D.
- `SaveManager`: Xử lý I/O dữ liệu JSON.
- `CameraManager`: Điều khiển chuyển động và hiệu ứng Camera.
- `CorePoolManager`: Quản lý tài nguyên bộ nhớ.

### 4. Hệ thống Lưu trữ JSON
Sử dụng thư viện `JsonUtility` của Unity để mã hóa dữ liệu thành file `.json`, đảm bảo tiến trình của người chơi được bảo toàn ngay cả khi thoát game đột ngột.

## 📂 Cấu trúc thư mục (Folder Structure)

- `_Scripts/Core`: Chứa các Manager chính và hệ thống nền tảng.
- `_Scripts/Player`: Xử lý logic di chuyển, tấn công và trạng thái của người chơi.
- `_Scripts/Enemy`: AI và logic hành vi của kẻ địch.
- `_Scripts/Weapons`: Hệ thống vũ khí trừu tượng (Abstraction).
- `_Scripts/Dungeon`: Logic tạo map và quản lý phòng.
- `_Prefabs`: Các đối tượng đã được đóng gói sẵn để tái sử dụng.

## 🎮 Hướng dẫn điều khiển (Controls)

- **WASD:** Di chuyển nhân vật.
- **Chuột trái:** Tấn công / Bắn súng.
- **Chuột phải (Dự kiến):** Dash (Lướt né đòn).
- **E:** Tương tác với Shop/Gacha/Cửa.
- **Esc:** Tạm dừng / Mở Menu.

## 🛠️ Cài đặt (Installation)

1. Clone repository này về máy.
2. Mở dự án bằng **Unity 2021.3 (LTS)** hoặc phiên bản mới hơn.
3. Mở cảnh `Menu` trong thư mục `Scenes` và nhấn Play.

---
*Dự án được phát triển bởi **Trung Đức** - Với mục tiêu học hỏi và áp dụng các kỹ thuật lập trình Game chuyên nghiệp.*
