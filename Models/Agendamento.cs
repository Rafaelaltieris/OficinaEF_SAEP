using System.ComponentModel.DataAnnotations;
namespace OficinaEF.Models;
 
public class Agendamento
{
    //Atributos
    public int Id{get;set;}
    [Required(ErrorMessage = "Informa a data e o horário.")]
    public DateTime? Data{get;set;}
    public string Servico{get;set;}
    public int EquipamentoId {get;set;}
    public Equipamento? Equipamento {get;set;}
}