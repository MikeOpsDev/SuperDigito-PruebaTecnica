using System.Collections.Generic;
using System.Linq;

namespace SuperDigitoApp.Services
{
    public class SuperDigitoService
    {
        public int Calcular(int x)
        {
            if (x < 0) throw new System.ArgumentException("No se permiten números negativos.", nameof(x));
            if (x < 10) return x;
            
            int sum = 0;
            while (x > 0)
            {
                sum += x % 10;
                x /= 10;
            }
            
            return Calcular(sum);
        }

        public string CalcularDetalle(int x)
        {
            if (x < 0) throw new System.ArgumentException("No se permiten números negativos.", nameof(x));
            if (x < 10) return $"SuperDigito({x})={x}";

            var detailSteps = new List<string>();
            detailSteps.Add($"SuperDigito({x})");

            int current = x;
            while (current >= 10)
            {
                var digits = new List<int>();
                foreach (char c in current.ToString())
                {
                    digits.Add(int.Parse(c.ToString()));
                }

                string sumStr = string.Join("+", digits);
                current = digits.Sum();

                detailSteps.Add($"SuperDigito({sumStr})");
                detailSteps.Add($"SuperDigito({current})");
            }
            detailSteps.Add(current.ToString() + ".");

            return string.Join("=", detailSteps);
        }
    }
}
