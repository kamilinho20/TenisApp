using System;
using System.ComponentModel.DataAnnotations;
using TenisApp.Core.Attribute;
using TenisApp.Core.Enum;
using TenisApp.Core.Model;

namespace TenisApp.Shared.ViewModel
{
    public class CourtViewModel : ObservableViewModel<CourtViewModel>
    {
        [Observable]
        [Required]
        public Court Court { get => GetValue<Court>(); set => SetValue(value); }
    }
}