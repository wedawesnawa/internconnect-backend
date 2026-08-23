using System.ComponentModel.DataAnnotations;

namespace InternconnectBackend.Helpers
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_extensions.Contains(extension))
                {
                    return new ValidationResult($"Hanya diperbolehkan file {string.Join(", ", _extensions)}.");
                }
            }
            return ValidationResult.Success;
        }
    }
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSize;
        public MaxFileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxSize)
                {
                    return new ValidationResult($"Ukuran file maksimal {_maxSize / (1024 * 1024)} MB.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
