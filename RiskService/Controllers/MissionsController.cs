using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using System.Threading.Tasks;

namespace RiskService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MissionsController : ControllerBase
    {
        private static readonly string[] Missions = new[]
        {
            "Capture Europe, Australia and a third continent of your choice",
            "Capture Europe, South America and a third continent of your choice",
            "Capture Asia and South America",
            "Capture Asia and Africa",
            "Capture North America and Australia",
            "Capture North America and Africa",
            "Capture 24 territories",
            "Capture 18 territories and occupy each with two troops",
            "Destroy all BLUE armies. If you are BLUE or another player kills BLUE first, then capture 24 territories",
            "Destroy all RED armies. If you are RED or another player kills RED first, then capture 24 territories",
            "Destroy all GREEN armies. If you are GREEN or another player kills GREEN first, then capture 24 territories",
            "Destroy all BLACK armies. If you are BLACK or another player kills BLACK first, then capture 24 territories",
            "Destroy all YELLOW armies. If you are YELLOW or another player kills YELLOW first, then capture 24 territories",
            "Destroy all PINK armies. If you are PINK or another player kills PINK first, then capture 24 territories"
        };

        private static readonly string CardBlank = "CardTemplate.png"; 

        private static readonly string[] CardImages = new[]
        {
            "Napoleon2.jpg",
            "Wellington2.jpg",
            "RedCoats2.jpg",
            "Infantry2.jpg",
            "Calvalry2.jpg",
            "Cannon2.jpg"
        };

        private readonly ILogger<MissionsController> _logger;

        private CardCreator cardCreator = new CardCreator(); 

        public MissionsController(ILogger<MissionsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return (GetText());
        }

        [HttpGet("text")]
        public string GetText()
        {
            var rng = new Random();
            return (Missions[rng.Next(Missions.Length)]);
        }

        [HttpGet("image64")]
        public string GetImage(string givenMission = "")
        {
            var rng = new Random();
            string missionText = givenMission;
            if (givenMission == "") missionText = Missions[rng.Next(Missions.Length)];

            string missionImage = CardImages[rng.Next(CardImages.Length)];
            string createCard64 = cardCreator.createMissionImage(CardBlank, missionImage, missionText);

            return (createCard64);
        }

        [HttpGet("json")]
        public IEnumerable<MissionCards> GetJson(int countMissions = 1)
        {
            var rng = new Random();
            string missionText = Missions[rng.Next(Missions.Length)];

            return Enumerable.Range(1, countMissions).Select(index => new MissionCards
            {
                Date = DateTime.Now,
                MissionText = missionText,
                MissionImage = GetImage(missionText)
            })
            .ToArray();
        }
    }
}
