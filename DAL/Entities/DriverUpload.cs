﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enums;

namespace DAL.Entities
{
    public class DriverUpload : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedDt { get; set; }
        public int UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("Driver")]
        [Required]
        public int DriverId { get; set; }
        [Required]
        public UploadType UploadType { get; set; }
        [Required]
        public string FilePath { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Description { get; set; }
        [Required]
        public bool IsApproved { get; set; }
        public string RejectionComment { get; set; }

        public virtual Driver Driver { get; set; }
        
    }
}