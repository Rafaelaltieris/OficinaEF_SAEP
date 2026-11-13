namespace OficinaEF.Models;
 
public class Equipamento
{
    //Atributos
    public int Id{get;set;}
    public string Tipo{get;set;}
    public string Modelo{get;set;}
    public int ClienteId {get;set;}
    public Cliente? Cliente {get;set;} //torna opcional
}