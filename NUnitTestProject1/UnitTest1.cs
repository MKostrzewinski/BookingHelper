using NUnit.Framework;
using BookingHelper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class Tests
    {
        private DateTime Before(DateTime dateTime, int days = 1)
        {
            return dateTime.AddDays(-days);
        }
        private DateTime After(DateTime dateTime, int days = 1)
        {
            return dateTime.AddDays(days);
        }
        private DateTime ArriveOn(int year, int month, int day)
        {
            return new DateTime(year, month, day, 14, 0, 0);
        }
        private DateTime DepartOn(int year, int month, int day)
        {
            return new DateTime(year, month, day, 10, 0, 0);
        }
        private Booking _exisitngBooking;
        private Mock<IBookingRepository> _repository;
        [SetUp]
        public void SetUp()
        {
            _exisitngBooking = new Booking
            {
                Id = 2,
                ArrivalDate = ArriveOn(2017, 1, 15),
                DepartureDate = DepartOn(2017, 1, 20),
                Reference = "a"
            };
            _repository = new Mock<IBookingRepository>();
            _repository.Setup(r => r.GetActiveBookings(1)).Returns(new List<Booking>{
                _exisitngBooking
            }.AsQueryable());
        }


        [Test]
        public void BookingStartsAndFinishesBeforeAnExistingBooking_ReturnEmptyString()
        {
            var result = BookingHelperHelper.OverlappingBookingsExist(new Booking
            {
                Id = 1,
                ArrivalDate = Before(_exisitngBooking.ArrivalDate, days: 2),
                DepartureDate = Before(_exisitngBooking.ArrivalDate)
            }, _repository.Object);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void BookingStartsBeforeAnExistingBookingButFinishingAfterStartOfIt()
        {
            var result = BookingHelperHelper.OverlappingBookingsExist(new Booking
            {
                Id = 1,
                ArrivalDate = Before(_exisitngBooking.ArrivalDate),
                DepartureDate = After(_exisitngBooking.ArrivalDate)
            }, _repository.Object);
            Assert.That(result, Is.EqualTo(_exisitngBooking.Reference));
        }

        [Test]
        public void BookingStartsAfterAnExistingBookingAndFinishesBeforeStartOfAnExistingBooking()
        {
            var result = BookingHelperHelper.OverlappingBookingsExist(new Booking
            {
                Id = 1,
                ArrivalDate = After(_exisitngBooking.ArrivalDate),
                DepartureDate = Before(_exisitngBooking.DepartureDate)
            }, _repository.Object);
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void BookingStartsBeforFinishOfExistingBokkingAndFinishAfterIt()
        {
            var result = BookingHelperHelper.OverlappingBookingsExist(new Booking
            {
                Id = 1,
                ArrivalDate = Before(_exisitngBooking.DepartureDate),
                DepartureDate = After(_exisitngBooking.DepartureDate)
            }, _repository.Object);
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void BookingStartsAfterFinishOfExistingBokkingAndFinishAfterIt()
        {
            var result = BookingHelperHelper.OverlappingBookingsExist(new Booking
            {
                Id = 1,
                ArrivalDate = After(_exisitngBooking.DepartureDate),
                DepartureDate = After(_exisitngBooking.DepartureDate)
            }, _repository.Object);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void BookingStartsBeforeStartOfExistingBookingAndFinishAfterEndOfIt()
        {
            var result = BookingHelperHelper.OverlappingBookingsExist(new Booking
            {
                Id = 1,
                ArrivalDate = Before(_exisitngBooking.ArrivalDate),
                DepartureDate = After(_exisitngBooking.DepartureDate)
            }, _repository.Object);
            Assert.That(result, Is.Not.Empty);
        }
    }
}