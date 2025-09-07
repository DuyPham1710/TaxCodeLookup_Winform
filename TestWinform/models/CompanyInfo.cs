using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWinform.models
{
    public class CompanyInfo
    {
        public string TaxID { get; set; }              // Mã số thuế
        public string Name { get; set; }               // Tên công ty
        public string TaxAuthority { get; set; }       // Địa chỉ thuế 
        public string Address { get; set; }            // Địa chỉ
        public string Status { get; set; }             // Tình trạng
        public string InternationalName { get; set; }  // Tên quốc tế
        public string ShortName { get; set; }          // Tên viết tắt
        public string Representative { get; set; }     // Người đại diện
        public string Telephone { get; set; }          // Số điện thoại
        public string FoundingDate { get; set; }       // Ngày hoạt động
        public string ManagingBy { get; set; }         // Quản lý bởi
        public string CompanyType { get; set; }        // Loại hình DN
        public string MainIndustry { get; set; }       // Ngành nghề chính

        public override string ToString()
        {
            return $"TaxID: {TaxID}, Name: {Name}, TaxAuthority: {TaxAuthority}, Address: {Address}, Status: {Status}, InternationalName: {InternationalName}, ShortName: {ShortName}, Representative: {Representative}, Telephone: {Telephone}, FoundingDate: {FoundingDate}, ManagingBy: {ManagingBy}, CompanyType: {CompanyType}, MainIndustry: {MainIndustry}";
        }
    }

}
