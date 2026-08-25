using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.Members
{
    public class CreateMemberDTOs
    {
        [Required(ErrorMessage ="Profile Picture Is Required")]
        [Display(Name="Profile Picture")]
        public IFormFile Photo { get; set; }
        [Required(ErrorMessage ="Name Is Required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage ="Name Can Only Contain a Letter")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage ="Invalid Email Format")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Is Required")]
        [Phone(ErrorMessage = "Invalid Phone Format")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(010|011|012|015)\d{8}$",ErrorMessage ="Invalid Phone Number")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Gender  Is Required")]

        public Gender Gender { get; set; }
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "City Is Required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Building Number Must Beetween 2 and 100")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City Can Only Contain a Letter")]
        public string City { get; set; }

        [Required(ErrorMessage = "Street Is Required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Street Number Must Beetween 2 and 100")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Street Can Only Contain a Letter And Number")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Date Of Birth Is Required")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage ="Health Record Is Required")]
        public HealthRecordViewModel HealthRecordViewModel { get; set; } = new();
        public string? Note { get;set; }


    }
}
