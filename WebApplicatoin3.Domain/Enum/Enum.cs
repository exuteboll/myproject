using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplicatoin3.Domain.Enum
{
    public class Enum
    {

    }
    public enum Role
    {
        [Display(Name = "Пользователь")]
        User = 0,
        [Display(Name = "Менеджер")]
        Manager = 1,
        [Display(Name = "Администратор")]
        Admin = 2,
    }
    public enum Status
    {
        [Description("Не рассмотрено")]
        NotConsidered = 0,
        [Description("В процессе")]
        InProgress = 1,
        [Description("Одобрено")]
        Approved = 2,
        [Description("Отклонено")]
        Denied = 3,
    }
    public enum OrderStatus
    {
        [Description("В корзине")]
        Cart = 0,

        [Description("Ожидает подтверждения")]
        Pending = 1,

        [Description("Подтвержден")]
        Confirmed = 2,

        [Description("Оплачен")]
        Paid = 3,

        [Description("Отправлен")]
        Shipped = 4,

        [Description("Доставлен")]
        Delivered = 5,

        [Description("Отменен")]
        Cancelled = 6,
    }
}
