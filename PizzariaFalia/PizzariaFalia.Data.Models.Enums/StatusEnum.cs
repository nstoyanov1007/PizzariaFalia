namespace PizzariaFalia.Data.Models.Enums
{
    public enum Status
    {
        Pending, //if order=pending -> serves as cart
        Ordered, //cart is emptied and the order is sent to the server
        Delivered,
        Cancelled
    }
}
