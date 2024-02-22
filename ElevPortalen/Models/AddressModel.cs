namespace ElevPortalen.Models
{
    public class AddressModel
    {
        public string? tekst { get; set; }
        public AddressDetails? adresse { get; set; }
    }

    public class AddressDetails
    {
        public string? id { get; set; }
        public int? status { get; set; }
        public int? darstatus { get; set; }
        public string? vejkode { get; set; }
        public string? vejnavn { get; set; }
        public string? adresseringsvejnavn { get; set; }
        public string? husnr { get; set; }
        public string? etage { get; set; }
        public string? dør { get; set; }
        public string? supplerendebynavn { get; set; }
        public string? postnr { get; set; }
        public string? postnrnavn { get; set; }
        public string? stormodtagerpostnr { get; set; }
        public string? stormodtagerpostnrnavn { get; set; }
        public string? kommunekode { get; set; }
        public string? adgangsadresseid { get; set; }
        public double? x { get; set; }
        public double? y { get; set; }
        public string? href { get; set; }
    }
}
