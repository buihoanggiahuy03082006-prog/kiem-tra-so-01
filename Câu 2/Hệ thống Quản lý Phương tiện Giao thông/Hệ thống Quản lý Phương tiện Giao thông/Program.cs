using System;
using System.Collections.Generic;
using System.Linq;

namespace LogisticsAutoSpeed
{
    // 1. Lớp trừu tượng Phương Tiện
    public abstract class PhuongTien
    {
        private string _maPT;
        private string _tenHang;
        private int _namSanXuat;
        private decimal _giaGoc;

        public string MaPT
        {
            get { return _maPT; }
            set { _maPT = string.IsNullOrWhiteSpace(value) ? "PT000" : value.Trim(); }
        }

        public string TenHang
        {
            get { return _tenHang; }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Tên hãng không được để trống.");
                _tenHang = value;
            }
        }

        public int NamSanXuat
        {
            get { return _namSanXuat; }
            set
            {
                if (value < 1900 || value > DateTime.Now.Year)
                    throw new ArgumentException("Năm sản xuất không hợp lệ!");
                _namSanXuat = value;
            }
        }

        public decimal GiaGoc
        {
            get { return _giaGoc; }
            set
            {
                if (value <= 0) throw new ArgumentException("Giá gốc phải lớn hơn 0.");
                _giaGoc = value;
            }
        }

        public PhuongTien(string maPT, string tenHang, int namSanXuat, decimal giaGoc)
        {
            MaPT = maPT;
            TenHang = tenHang;
            NamSanXuat = namSanXuat;
            GiaGoc = giaGoc;
        }

        public abstract decimal TinhGiaLanBanh();

        public virtual string GetInfo()
        {
            return $"Mã: {MaPT} | Hãng: {TenHang} | Năm SX: {NamSanXuat} | Giá gốc: {GiaGoc:N0} VNĐ";
        }
    }

    // 2. Lớp Ô tô kế thừa Phương Tiện
    public class OTo : PhuongTien
    {
        private int _soChoNgoi;
        private double _dungTichDongCo;

        public int SoChoNgoi
        {
            get { return _soChoNgoi; }
            set
            {
                if (value <= 0) throw new ArgumentException("Số chỗ ngồi phải > 0");
                _soChoNgoi = value;
            }
        }

        public double DungTichDongCo
        {
            get { return _dungTichDongCo; }
            set
            {
                if (value <= 0) throw new ArgumentException("Dung tích động cơ phải > 0");
                _dungTichDongCo = value;
            }
        }

        public OTo(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int soChoNgoi, double dungTichDongCo)
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            SoChoNgoi = soChoNgoi;
            DungTichDongCo = dungTichDongCo;
        }

        public override decimal TinhGiaLanBanh()
        {
            if (SoChoNgoi <= 9)
            {
                return GiaGoc + (0.12m * GiaGoc) + (0.30m * GiaGoc);
            }
            return GiaGoc + (0.10m * GiaGoc);
        }

        public override string GetInfo()
        {
            return base.GetInfo() + $" | Số chỗ: {SoChoNgoi} | Dung tích ĐC: {DungTichDongCo}L";
        }
    }

    // 3. Lớp Xe máy kế thừa Phương Tiện
    public class XeMay : PhuongTien
    {
        public int DungTichXylanh { get; set; }

        public XeMay(string maPT, string tenHang, int namSanXuat, decimal giaGoc, int dungTichXylanh)
            : base(maPT, tenHang, namSanXuat, giaGoc)
        {
            DungTichXylanh = dungTichXylanh;
        }

        public override decimal TinhGiaLanBanh()
        {
            if (DungTichXylanh < 175)
            {
                return GiaGoc + (0.02m * GiaGoc);
            }
            return GiaGoc + (0.05m * GiaGoc);
        }

        public override string GetInfo()
        {
            return base.GetInfo() + $" | Dung tích xilanh: {DungTichXylanh}cc";
        }
    }

    // 4. Lớp Quản lý Phương Tiện
    public class QuanLyPhuongTien
    {
        private List<PhuongTien> danhSachPT = new List<PhuongTien>();

        public void AddPhuongTien(PhuongTien pt)
        {
            danhSachPT.Add(pt);
        }

        public void DisplayAll()
        {
            if (danhSachPT.Count == 0)
            {
                Console.WriteLine("Danh sách trống!");
                return;
            }
            foreach (var pt in danhSachPT)
            {
                Console.WriteLine($"{pt.GetInfo()} => Giá lăn bánh: {pt.TinhGiaLanBanh():N0} VNĐ");
            }
        }

        public PhuongTien FindMaxGiaLanBanh()
        {
            if (danhSachPT.Count == 0) return null;
            return danhSachPT.OrderByDescending(pt => pt.TinhGiaLanBanh()).FirstOrDefault();
        }

        public List<PhuongTien> SearchByName(string keyword)
        {
            return danhSachPT.Where(pt => pt.TenHang != null && pt.TenHang.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }
    }

    // 5. Lớp Program (Chứa Menu chính)
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            QuanLyPhuongTien qlpt = new QuanLyPhuongTien();
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n========== HỆ THỐNG QUẢN LÝ PHƯƠNG TIỆN ==========");
                Console.WriteLine("1. Thêm mới Ô tô");
                Console.WriteLine("2. Thêm mới Xe máy");
                Console.WriteLine("3. Hiển thị danh sách phương tiện");
                Console.WriteLine("4. Tìm phương tiện có Giá lăn bánh cao nhất");
                Console.WriteLine("5. Tìm kiếm phương tiện theo Tên hãng");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn chức năng (0-5): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ThemOTo(qlpt);
                        break;
                    case "2":
                        ThemXeMay(qlpt);
                        break;
                    case "3":
                        Console.WriteLine("\n--- Danh Sách Phương Tiện ---");
                        qlpt.DisplayAll();
                        break;
                    case "4":
                        Console.WriteLine("\n--- Phương Tiện Giá Lăn Bánh Cao Nhất ---");
                        var maxPt = qlpt.FindMaxGiaLanBanh();
                        if (maxPt != null)
                            Console.WriteLine($"Kết quả: {maxPt.GetInfo()} => Giá lăn bánh: {maxPt.TinhGiaLanBanh():N0} VNĐ");
                        else
                            Console.WriteLine("Danh sách trống!");
                        break;
                    case "5":
                        Console.Write("\nNhập tên hãng cần tìm: ");
                        string keyword = Console.ReadLine();
                        var result = qlpt.SearchByName(keyword);
                        if (result.Count > 0)
                        {
                            Console.WriteLine($"\n--- Kết quả tìm kiếm '{keyword}' ---");
                            foreach (var pt in result)
                                Console.WriteLine($"{pt.GetInfo()} => Giá lăn bánh: {pt.TinhGiaLanBanh():N0} VNĐ");
                        }
                        else
                        {
                            Console.WriteLine($"Không tìm thấy phương tiện nào của hãng '{keyword}'.");
                        }
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Đã thoát chương trình.");
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ, vui lòng thử lại!");
                        break;
                }
            }
        }

        static void ThemOTo(QuanLyPhuongTien qlpt)
        {
            Console.WriteLine("\n--- Nhập thông tin Ô Tô ---");
            try
            {
                Console.Write("Nhập mã phương tiện: ");
                string maPT = Console.ReadLine();

                Console.Write("Nhập tên hãng: ");
                string tenHang = Console.ReadLine();

                Console.Write("Nhập năm sản xuất: ");
                int namSX = int.Parse(Console.ReadLine());

                Console.Write("Nhập giá gốc (VNĐ): ");
                decimal giaGoc = decimal.Parse(Console.ReadLine());

                Console.Write("Nhập số chỗ ngồi: ");
                int soChoNgoi = int.Parse(Console.ReadLine());

                Console.Write("Nhập dung tích động cơ (Lít, VD: 2.0): ");
                double dungTich = double.Parse(Console.ReadLine());

                // Khởi tạo đối tượng, bắt lỗi chuẩn theo yêu cầu đề
                OTo oto = new OTo(maPT, tenHang, namSX, giaGoc, soChoNgoi, dungTich);

                qlpt.AddPhuongTien(oto);
                Console.WriteLine("=> Thêm Ô tô thành công!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n=> Hệ thống ném ngoại lệ ArgumentException(\"{ex.Message}\").");
                Console.WriteLine("=> Không cho tạo đối tượng.");
            }
            catch (Exception)
            {
                Console.WriteLine($"\n=> LỖI: Dữ liệu nhập vào sai định dạng (vui lòng nhập số cho Năm SX, Giá gốc...)");
            }
        }

        static void ThemXeMay(QuanLyPhuongTien qlpt)
        {
            Console.WriteLine("\n--- Nhập thông tin Xe Máy ---");
            try
            {
                Console.Write("Nhập mã phương tiện: ");
                string maPT = Console.ReadLine();

                Console.Write("Nhập tên hãng: ");
                string tenHang = Console.ReadLine();

                Console.Write("Nhập năm sản xuất: ");
                int namSX = int.Parse(Console.ReadLine());

                Console.Write("Nhập giá gốc (VNĐ): ");
                decimal giaGoc = decimal.Parse(Console.ReadLine());

                Console.Write("Nhập dung tích xilanh (cc, VD: 150): ");
                int dungTich = int.Parse(Console.ReadLine());

                // Khởi tạo đối tượng, bắt lỗi chuẩn theo yêu cầu đề
                XeMay xemay = new XeMay(maPT, tenHang, namSX, giaGoc, dungTich);

                qlpt.AddPhuongTien(xemay);
                Console.WriteLine("=> Thêm Xe máy thành công!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n=> Hệ thống ném ngoại lệ ArgumentException(\"{ex.Message}\").");
                Console.WriteLine("=> Không cho tạo đối tượng.");
            }
            catch (Exception)
            {
                Console.WriteLine($"\n=> LỖI: Dữ liệu nhập vào sai định dạng (vui lòng nhập số cho Năm SX, Giá gốc...)");
            }
        }
    }
}