﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.Application.Queries.TransactionQueries
{
    public class TransactionDetailsDto
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public decimal WalletBalance { get; set; }

        public Guid UserId { get; set; }
        public string Fullname { get; set; }

        public string Note { get; set; }
        public decimal Amount { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public TransactionTypeEnum TransactionType { get; set; }
        public EntityStatus Status { get; set; }
        public string TransactionReference { get; set; }
        public string Description { get; set; }
        public string TrackCode { get; set; }

        public string TransactionVerificationId { get; set; }
    }
}
