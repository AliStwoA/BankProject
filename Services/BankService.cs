using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Q2_Bank.Models
{
    public class BankService
    {
        private readonly BankDbContext _context;

        public BankService(BankDbContext context)
        {
            _context = context;
        }

        // احراز هویت کارت
        public string AuthenticateUser(string cardNumber, string password)
        {
            var card = _context.Cards.FirstOrDefault(c => c.CardNumber == cardNumber);

            if (card == null)
                return "Card not found.";

            if (!card.IsActive)
                return "Card is blocked.";

            if (card.Password != password)
            {
                card.FailedAttempts++;

                if (card.FailedAttempts >= 3)
                {
                    card.IsActive = false;
                    _context.SaveChanges();
                    return "Card is blocked due to multiple incorrect attempts.";
                }

                _context.SaveChanges();
                return $"Incorrect password. {3 - card.FailedAttempts} attempts remaining.";
            }

            // Reset failed attempts on successful login
            card.FailedAttempts = 0;
            _context.SaveChanges();

            return "Authentication successful.";
        }

        // انتقال وجه
        public string TransferWithRollback(string sourceCardNumber, string destinationCardNumber, float amount)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var sourceCard = _context.Cards.FirstOrDefault(c => c.CardNumber == sourceCardNumber);
                var destinationCard = _context.Cards.FirstOrDefault(c => c.CardNumber == destinationCardNumber);

                if (sourceCard == null || destinationCard == null)
                    return "Invalid card numbers.";

                if (!sourceCard.IsActive)
                    return "Source card is blocked.";

                if (sourceCard.Balance < amount)
                    return "Insufficient balance.";

                sourceCard.Balance -= amount;
                destinationCard.Balance += amount;

                var newTransaction = new Transaction
                {
                    SourceCardNumber = sourceCardNumber,
                    DestinationCardNumber = destinationCardNumber,
                    Amount = amount,
                    TransactionDate = DateTime.Now,
                    IsSuccessful = true
                };

                _context.Transactions.Add(newTransaction);
                _context.SaveChanges();

                transaction.Commit();
                return "Transfer successful.";
            }
            catch
            {
                transaction.Rollback();
                return "Transfer failed. Please try again.";
            }
        }

        // دریافت لیست تراکنش‌ها
        public List<Transaction> GetTransactions(string cardNumber)
        {
            return _context.Transactions
                .Where(t => t.SourceCardNumber == cardNumber || t.DestinationCardNumber == cardNumber)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
        }

        // دریافت موجودی کارت
        public float GetBalance(string cardNumber)
        {
            var card = _context.Cards.FirstOrDefault(c => c.CardNumber == cardNumber);

            if (card == null)
                throw new ArgumentException("Card not found.");

            return card.Balance;
        }
    }
}