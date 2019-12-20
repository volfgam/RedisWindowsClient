using System;

namespace RedisWindowsClient.Models
{
    public class EmissorToken
    {
        public string Id { get; set; }
        public int IdGov { get; set; }
        public string Valor { get; set; }
        public DateTime Validade { get; set; }
        public bool Inativo { get; set; }
        public int IdProduto { get; set; }
    }
}