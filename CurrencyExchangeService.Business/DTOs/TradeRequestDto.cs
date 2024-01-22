using System.ComponentModel.DataAnnotations;

namespace CurrencyExchangeService.Business.DTOs
{
    public class TradeRequestDto
    {
        [Required(ErrorMessage = "Client ID is required.")]
        [StringLength(50, ErrorMessage = "Client ID cannot exceed 50 characters.")]
        public string ClientId { get; set; }

        [Required(ErrorMessage = "Base currency is required.")]
        [StringLength(3, ErrorMessage = "Base currency code must be 3 characters.")]
        public string BaseCurrency { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount is required.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Target currency is required.")]
        [MinLength(3, ErrorMessage = "Target currency code atleast 3 characters.")]
        public string TargetCurrency { get; set; }
    }
}
