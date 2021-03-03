using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Casino_Royale_Api.Entities
{
    [Table("players")]
    public class Player
    {
        public static Player DefaultPlayerFactory(string username)
        {
            double startingMoney = 100.00;
            double startingTotalEarned = 0.00;
            double startingLostEarned = 0.00;
            bool playerActive = false;

            return new Player(username, startingMoney, startingTotalEarned, startingLostEarned, playerActive);
        }

        #nullable enable
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("currentMoney")]
        public double? CurrentMoney { get; set; }
        [Column("totalEarned")]
        public double? TotalEarned { get; set; }
        [Column("totalLost")]
        public double? TotalLost { get; set; }
        [Column("active")]
        public bool? Active { get; set; }
        [Column("lastUpdated")]
        public DateTime? LastUpdated { get; set; }

        public Player(string username, double? currentMoney, double? totalEarned, double? totalLost, bool? active)
        {
            this.Username = username;
            this.CurrentMoney = currentMoney;
            this.TotalEarned = totalEarned;
            this.TotalLost = totalLost;
            this.Active = active;
        }

        public Player()
        {
        }
    }
}
