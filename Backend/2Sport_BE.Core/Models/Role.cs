using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.Repository.Models;

[Table("Roles")]
public class Role
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("RoleName", TypeName = "nvarchar")]
    [MaxLength(50)]
    public string RoleName { get; set; }

    [Column("Description", TypeName = "nvarchar")]
    [MaxLength(255)]
    public string Description { get; set; }

    [DisplayFormat(DataFormatString = "{0:HH-mm-ss:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.DateTime)]
    public DateTime? CreateAt { get; set; }

    public virtual ICollection<User> Users { get; set; }

    //Create Indexer
    public object this[string name]
    {
        get
        {
            if (name.ToUpper() == "ID")
                return Id;
            else if (name.ToUpper() == "ROLENAME")
                return RoleName;
            else if (name.ToUpper() == "DESCRIPTION")
                return Description;
            else if (name.ToUpper() == "CREATEDAT")
                return CreateAt;
            else return null;
        }
        set
        {
            if (name.ToUpper() == "ID")
                Id = Convert.ToInt32(value);
            else if (name.ToUpper() == "ROLENAME")
                RoleName = value.ToString();
            else if (name.ToUpper() == "DESCRIPTION")
                Description = value.ToString();
            else if (name.ToUpper() == "CREATEDAT")
                CreateAt = DateTime.Parse(value.ToString());
        }
    }
}
