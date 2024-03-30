using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Navigation
{
    public partial class Menu : IValidatableObject
    {
        // Properties
        public string ParentMenuId { get; set; }

        public string MenuId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public int Priority { get; set; }

        public string ActionUri { get; set; }

        public string ResourceUri { get; set; }
    }

    public partial class Menu : IValidatableObject
    {
        // Methods
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            #region Contracts

            if (validationContext == null) throw new ArgumentException($"{nameof(validationContext)}=null");

            #endregion

            // ParentMenuId

            // MenuId
            if (string.IsNullOrEmpty(this.MenuId) == true) yield return new ValidationResult($"{nameof(this.MenuId)}=null", new[] { nameof(this.MenuId) });

            // Name
            if (string.IsNullOrEmpty(this.Name) == true) yield return new ValidationResult($"{nameof(this.Name)}=null", new[] { nameof(this.Name) });

            // Description

            // Icon

            // Priority

            // ActionUri

            // ResourceUri
            if (string.IsNullOrEmpty(this.ResourceUri) == true) yield return new ValidationResult($"{nameof(this.ResourceUri)}=null", new[] { nameof(this.ResourceUri) });
            if (this.ResourceUri.Split("/").Length < 3) yield return new ValidationResult($"{nameof(this.ResourceUri)}.Length<3", new[] { nameof(this.ResourceUri) });
        }
    }
}
