namespace GoodHamburger.Domain.Ordering.Enums;

public enum OrderStatus
{
    Pending = 1,        
    Confirmed = 2,      
    InPreparation = 3,  
    Ready = 4,          
    InDelivery = 5,     
    Completed = 6,      
    Cancelled = 7,      
    Failed = 8
}