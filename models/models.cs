using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ConferenceRoomBookingAPI.Models
{
    /// Модель зала
    public class Hall
    {
        /// ідентифікатор послуги
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Это должно указать, что Id генерируется автоматически
        public int Id { get; set; }

        /// назва зала
        public string Name { get; set; }

        /// місткість зала
        public int Capacity { get; set; }

        /// базова ціна за годину
        public decimal BasePricePerHour { get; set; }

        /// список послуг даного зала
        public ICollection<HallService> HallServices { get; set; }

        /// список бронювань даного зала
        public ICollection<Booking> Bookings { get; set; }
    }

    /// Модель послуги
    public class Service
    {
        /// ідентифікатор послуги
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Это должно указать, что Id генерируется автоматически
        public int Id { get; set; }
    

        /// назва послуги
        public string Name { get; set; }

        /// ціна послуги
        public decimal Price { get; set; }

        /// список залів, у яких надається дана послуга
        public ICollection<HallService> HallServices { get; set; }

        /// список бронювань, у яких додана дана послуга
        public ICollection<BookingService> BookingServices { get; set; }
    }

    /// Модель бронювання
    public class Booking
    {
        /// ідентифікатор бронювання
        public int Id { get; set; }

        /// ідентифікатор зала, у якому буде проведено бронювання
        public int HallId { get; set; }

        /// зал, у якому буде проведено бронювання
        public Hall Hall { get; set; }

        /// дата проведення бронювання
        public DateTime Date { get; set; }

        /// час початку бронювання
        public TimeSpan StartTime { get; set; }

        /// час закінчення бронювання
        public TimeSpan EndTime
        {
            get
            {
                return StartTime.Add(TimeSpan.FromHours(Duration));
            }

            set
            {
                Duration = (int)value.Subtract(StartTime).TotalHours;
            }
        }

        /// тривалість бронювання
        public int Duration { get; set; }

        /// загальна ціна бронювання
        public decimal TotalPrice { get; set; }

        /// список послуг, доданих до бронювання
        public ICollection<BookingService> BookingServices { get; set; }
    }

    /// Модель зв язку зала і послуги
    public class HallService
    {
        /// ідентифікатор послуги
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Это должно указать, что Id генерируется автоматически
        public int Id { get; set; }

        /// ідентифікатор зала
        public int HallId { get; set; }

        /// зал, у якому надається послуга
        public Hall Hall { get; set; }

        /// ідентифікатор послуги
        public int ServiceId { get; set; }

        /// послуга, яка надається у залі
        public Service Service { get; set; }
    }

    /// Модель зв язку бронювання і послуги
    public class BookingService
    {
        /// ідентифікатор послуги
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Это должно указать, что Id генерируется автоматически
        public int Id { get; set; }
        /// ідентифікатор бронювання
        public int BookingId { get; set; }

        /// бронювання, до якого додається послуга
        public Booking Booking { get; set; }

        /// ідентифікатор послуги
        public int ServiceId { get; set; }

        /// послуга, яка додається до бронювання
        public Service Service { get; set; }
    }
    
        public class CreateHallDto
        {
            public string Name { get; set; }
            public int Capacity { get; set; }
            public decimal HourlyRate { get; set; }
            public List<int> ServiceIds { get; set; } // список ID услуг, которые клиент выбирает
        }

        public class UpdateHallRequest
        {
            public string Name { get; set; }
            public int Capacity { get; set; }
            public decimal HourlyRate { get; set; }
            public List<int> ServiceIds { get; set; }
        }

        public class BookingRequest
        {
            public int HallId { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan StartTime { get; set; }
            public int Duration { get; set; } // В часах
            public List<int> ServiceIds { get; set; }
        }

        public class SearchAvailableHallsRequest
        {
            public DateTime Date { get; set; } // Дата бронирования
            public TimeSpan StartTime { get; set; } // Время начала бронирования
            public TimeSpan EndTime { get; set; } // Время окончания бронирования
            public int RequiredCapacity { get; set; } // Требуемая вместимость
        }






}
