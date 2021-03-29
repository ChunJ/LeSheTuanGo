using LeSheTuanGo.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeSheTuanGo.ViewModels
{
    public class MemberViewModel
    {
        private Member iv_Member = null;
        public Member member { get { return iv_Member; } }
        public MemberViewModel()
        {
            iv_Member = new Member();
        }
        public MemberViewModel(Member p)
        {
            iv_Member = p;
        }

        //public Member()
        //{
        //    GarbageServiceOffers = new HashSet<GarbageServiceOffer>();
        //    GarbageServiceUseRecords = new HashSet<GarbageServiceUseRecord>();
        //    OrderBuyRecords = new HashSet<OrderBuyRecord>();
        //    Orders = new HashSet<Order>();
        //}

        public int MemberId { get { return iv_Member.MemberId; } set { iv_Member.MemberId = value; } }

        [DisplayName("姓")]
        [Required(ErrorMessage = "不能為空")]
        public string FirstName { get {return iv_Member.FirstName; } set {iv_Member.FirstName = value; } }

        [DisplayName("名")]
        [Required(ErrorMessage = "不能為空")]
        public string LastName { get {return iv_Member.LastName; } set {iv_Member.LastName = value; } }

        [DisplayName("電子信箱")]
        [Required(ErrorMessage = "不能為空")]
        public string Email { get {return iv_Member.Email; } set {iv_Member.Email = value; } }

        [DisplayName("密碼")]
        [Required(ErrorMessage = "不能為空")]
        public string Password { get {return iv_Member.Password; } set {iv_Member.Password = value; } }

        public string PasswordSalt { get {return iv_Member.PasswordSalt; } set {iv_Member.PasswordSalt = value; } }

        public short DistrictId { get {return iv_Member.DistrictId; } set {iv_Member.DistrictId = value; } }

        [DisplayName("地址")]
        [Required(ErrorMessage = "不能為空")]
        public string Address { get {return iv_Member.Address; } set { iv_Member.Address = value; } }

        [DisplayName("緯度")]
        public decimal Latitude { get { return iv_Member.Latitude; } set { iv_Member.Latitude = value; } }

        [DisplayName("經度")]
        public decimal Longitude { get { return iv_Member.Longitude; } set { iv_Member.Longitude = value; } }

        [DisplayName("餘額")]
        public int Balance { get { return iv_Member.Balance; } set { iv_Member.Balance = value; } }

        public string ProfileImagePath { get { return iv_Member.ProfileImagePath; } set { iv_Member.ProfileImagePath = value; } }
<<<<<<< Updated upstream
       
        [DisplayName("生日")]
=======
>>>>>>> Stashed changes
        public DateTime DateOfBirth { get { return iv_Member.DateOfBirth; } set { iv_Member.DateOfBirth = value; } }

        [DisplayName("照片")]

        public IFormFile image { get; set; }

        //
        public virtual DistrictRef District { get; set; }
        public virtual ICollection<GarbageServiceOffer> GarbageServiceOffers { get; set; }
        public virtual ICollection<GarbageServiceUseRecord> GarbageServiceUseRecords { get; set; }
        public virtual ICollection<OrderBuyRecord> OrderBuyRecords { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

    }
}
