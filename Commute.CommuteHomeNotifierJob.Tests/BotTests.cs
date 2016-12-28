using System;
using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Lilybot.Commute.Application;
using Lilybot.Commute.CommuteHomeNotifierJob;
using Lilybot.Commute.CommuteHomeNotifierJob.Models;
using Lilybot.Commute.Domain;
using Lilybot.Positioning.CommonTypes;
using NUnit.Framework;

namespace Commute.CommuteHomeNotifierJob.Tests
{
    [TestFixture]
    public class BotTests
    {
        private ISlackMessageSender _slackMessageSender;
        private IFamilyProfileRepository _familyProfileRepository;
        private IStateRepository _stateRepository;
        private Bot _sut;
        private FamilyState _currentState;

        private static readonly Guid FAMILY_ID = Guid.Parse("66489C66-C4FB-454C-88BE-0CD609B1CB1F");
        private const string PIM = "10153830410348370";
        private const string ELINA = "1136574873066943";

        [SetUp]
        protected void SetUp()
        {
            _slackMessageSender = A.Fake<ISlackMessageSender>();
            _familyProfileRepository = A.Fake<IFamilyProfileRepository>();
            _stateRepository = A.Fake<IStateRepository>();

            A.CallTo(() => _stateRepository.SaveState(A<FamilyState>._)).Invokes((FamilyState state) => _currentState = state);
            A.CallTo(() => _familyProfileRepository.GetByFacebookUserId(A<string>._)).Returns(CreateStridFamily());
            _sut = new Bot(_familyProfileRepository, _stateRepository, _slackMessageSender);
        }


        [TestCase("2016-10-22 06:23:00")] // SATURDAY
        [TestCase("2016-10-23 19:00:00")] // SUNDAY
        public void ProcessHotspotEvent_ShouldDoNothing_OnWeekends(string date)
        {
            // ARRANGE
            var hotspotEvent = new HotspotEventMessage(DateTimeOffset.Parse(date), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<HotspotType>(), A.Dummy<ActionType>());
            _currentState = null;

            // ACT
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _familyProfileRepository.GetByFacebookUserId(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).MustNotHaveHappened();
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
        }


        //=== SCENARIO 1: Pim hämtar, Elina lämnar, båda rapporterar position "by the book"
        [Test]
        public void SC1_ProcessHotspotEvent_WhenElinaLeavesHome()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(null);

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 06:23:00"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _familyProfileRepository.GetByFacebookUserId(ELINA)).MustHaveHappened();
            A.CallTo(() => _stateRepository.GetState(FAMILY_ID)).MustHaveHappened();
            A.CallTo(() => _stateRepository.SaveState(A<FamilyState>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayToWork, hotspotEvent.Timestamp),
                    new FamilyState.Member(PIM)
                }
            });
        }

        [Test]
        public void SC1_ProcessHotspotEvent_WhenElinaArrivesAtWork()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayToWork, DT("2016-10-19 06:23:00")),
                    new FamilyState.Member(PIM)
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 06:45:00"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _familyProfileRepository.GetByFacebookUserId(ELINA)).MustHaveHappened();
            A.CallTo(() => _stateRepository.GetState(FAMILY_ID)).MustHaveHappened();
            A.CallTo(() => _stateRepository.SaveState(A<FamilyState>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtWork, hotspotEvent.Timestamp),
                    new FamilyState.Member(PIM)
                }
            });
        }

        [Test]
        public void SC1_ProcessHotspotEvent_WhenPimLeavesHome()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtWork, DT("2016-10-19 06:45:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 07:23:00"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _familyProfileRepository.GetByFacebookUserId(PIM)).MustHaveHappened();
            A.CallTo(() => _stateRepository.GetState(FAMILY_ID)).MustHaveHappened();
            A.CallTo(() => _stateRepository.SaveState(A<FamilyState>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtWork, DT("2016-10-19 06:45:00")),
                    new FamilyState.Member(PIM, MemberState.OnWayToWork, hotspotEvent.Timestamp)
                }
            });
        }

        [Test]
        public void SC1_ProcessHotspotEvent_WhenPimArrivesAtWork()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtWork, DT("2016-10-19 06:45:00")),
                    new FamilyState.Member(PIM, MemberState.OnWayToWork, DT("2016-10-19 07:23:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 08:25:00"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _familyProfileRepository.GetByFacebookUserId(PIM)).MustHaveHappened();
            A.CallTo(() => _stateRepository.GetState(FAMILY_ID)).MustHaveHappened();
            A.CallTo(() => _stateRepository.SaveState(A<FamilyState>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtWork, DT("2016-10-19 06:45:00")),
                    new FamilyState.Member(PIM, MemberState.AtWork, hotspotEvent.Timestamp)
                }
            });
        }

        [Test]
        public void SC1_ProcessHotspotEvent_WhenElinaLeavesWork()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtWork, DT("2016-10-19 06:45:00")),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 08:25:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 15:15:00"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _familyProfileRepository.GetByFacebookUserId(ELINA)).MustHaveHappened();
            A.CallTo(() => _stateRepository.GetState(FAMILY_ID)).MustHaveHappened();
            A.CallTo(() => _stateRepository.SaveState(A<FamilyState>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayHome, hotspotEvent.Timestamp),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 08:25:00"))
                }
            });
        }

        [Test]
        public void SC1_ProcessHotspotEvent_WhenElinaArrivesAtHome()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayHome, DT("2016-10-19 15:15:00")),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 08:25:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 16:10:00"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _familyProfileRepository.GetByFacebookUserId(ELINA)).MustHaveHappened();
            A.CallTo(() => _stateRepository.GetState(FAMILY_ID)).MustHaveHappened();
            A.CallTo(() => _stateRepository.SaveState(A<FamilyState>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, hotspotEvent.Timestamp),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 08:25:00"))
                }
            });
        }

        [Test]
        public void SC1_ProcessHotspotEvent_WhenPimLeavesWork()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 16:10:00")),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 08:25:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 17:35:00"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _familyProfileRepository.GetByFacebookUserId(PIM)).MustHaveHappened();
            A.CallTo(() => _stateRepository.GetState(FAMILY_ID)).MustHaveHappened();
            A.CallTo(() => _stateRepository.SaveState(A<FamilyState>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 16:10:00")),
                    new FamilyState.Member(PIM, MemberState.OnWayHome, hotspotEvent.Timestamp)
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }

        [Test]
        public void SC1_ProcessHotspotEvent_WhenPimArrivesAtHome()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 16:10:00")),
                    new FamilyState.Member(PIM, MemberState.OnWayHome, DT("2016-10-19 17:35:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 18:05:00"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _familyProfileRepository.GetByFacebookUserId(PIM)).MustHaveHappened();
            A.CallTo(() => _stateRepository.GetState(FAMILY_ID)).MustHaveHappened();
            A.CallTo(() => _stateRepository.SaveState(A<FamilyState>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 16:10:00")),
                    new FamilyState.Member(PIM, MemberState.AtHome, hotspotEvent.Timestamp)
                }
            });
        }


        //=== SCENARIO 2: Pim lämnar, Elina rapporterar inte position
        [Test]
        public void SC2_ProcessHotspotEvent_WhenPimLeavesHome()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(null);

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 07:23:00"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.OnWayToWork, hotspotEvent.Timestamp)
                }
            });
        }

        [Test]
        public void SC2_ProcessHotspotEvent_WhenPimLeavesWork()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 08:25:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 17:35:00"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.OnWayHome, hotspotEvent.Timestamp)
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 3: Pim hämtar, Elina rapporterar inte position
        [Test]
        public void SC3_ProcessHotspotEvent_WhenPimLeavesHome()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(null);

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 07:00:00"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.OnWayToWork, hotspotEvent.Timestamp)
                }
            });
        }

        [Test]
        public void SC3_ProcessHotspotEvent_WhenPimLeavesWork()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 07:40:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 15:35:00"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.OnWayHome, hotspotEvent.Timestamp)
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 4: Pim jobbar, Elina ledig och hemma hela dagen
        [Test]
        public void SC4_ProcessHotspotEvent_WhenPimLeavesHome()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(null);

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 06:45:00"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.OnWayToWork, hotspotEvent.Timestamp)
                }
            });
        }

        [Test]
        public void SC4_ProcessHotspotEvent_WhenPimLeavesWork()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 07:25:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 16:45:00"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.OnWayHome, hotspotEvent.Timestamp)
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 5: Pim jobbar, Elina ledig och iväg runt lunch
        [Test]
        public void SC5_ProcessHotspotEvent_WhenElinaLeavesHomeBeforeLunch()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 07:30:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 10:45:00"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.Unknown, hotspotEvent.Timestamp),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 07:30:00"))
                }
            });
        }

        [Test]
        public void SC5_ProcessHotspotEvent_WhenElinaArriveAtHomeAfterLunch()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.Unknown, DT("2016-10-19 10:45:00")),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 07:30:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 13:00:00"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, hotspotEvent.Timestamp),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 07:30:00"))
                }
            });
        }

        [Test]
        public void SC5_ProcessHotspotEvent_WhenPimLeavesWork()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 13:00:00")),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 07:30:00"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 16:45:00"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 13:00:00")),
                    new FamilyState.Member(PIM, MemberState.OnWayHome, hotspotEvent.Timestamp)
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 6: Pim lämnar, Elina på Hällered samt hämtar
        [Test]
        public void SC6_ProcessHotspotEvent_WhenPimLeavesHome()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayToWork, DT("2016-10-19 06:00:00")),
                    new FamilyState.Member(PIM)
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 07:30"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayToWork, DT("2016-10-19 06:00:00")),
                    new FamilyState.Member(PIM, MemberState.OnWayToWork, DT("2016-10-19 07:30"))
                }
            });
        }

        [Test]
        public void SC6_ProcessHotspotEvent_WhenElinaArrivesAtHome()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayToWork, DT("2016-10-19 06:00")),
                    new FamilyState.Member(PIM, MemberState.OnWayToWork, DT("2016-10-19 07:30"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 15:55"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 15:55")),
                    new FamilyState.Member(PIM, MemberState.OnWayToWork, DT("2016-10-19 07:30"))
                }
            });
        }

        [Test]
        public void SC6_ProcessHotspotEvent_WhenPimLeavesWork()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 15:55")),
                    new FamilyState.Member(PIM, MemberState.OnWayToWork, DT("2016-10-19 07:30"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 17:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 15:55")),
                    new FamilyState.Member(PIM, MemberState.OnWayHome, DT("2016-10-19 17:35"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 7: Pim hämtar, Elina på Hällered samt lämnar
        [Test]
        public void SC7_ProcessHotspotEvent_WhenPimLeavesWork()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayToWork, DT("2016-10-19 07:45")),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 08:25"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 15:30"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayToWork, DT("2016-10-19 07:45")),
                    new FamilyState.Member(PIM, MemberState.OnWayHome, DT("2016-10-19 15:30"))
                }
            });
        }

        [Test]
        public void SC7_ProcessHotspotEvent_WhenPimArrivesAtHome()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayToWork, DT("2016-10-19 07:45")),
                    new FamilyState.Member(PIM, MemberState.OnWayHome, DT("2016-10-19 15:30"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayToWork, DT("2016-10-19 07:45")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:15"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }

        [Test]
        public void SC7_ProcessHotspotEvent_WhenElinaArrivesAtHome()
        {
            // ARRANGE
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.OnWayToWork, DT("2016-10-19 07:45")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:15"))
                }
            });

            // ACT
            var hotspotEvent = new HotspotEventMessage(DT("2016-10-19 18:35"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter);
            _sut.ProcessHotspotEvent(hotspotEvent, A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 18:35")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:15"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 8: Pim är på gymmet och hämtar, Elina lämnar
        [Test]
        public void SC8_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 05:35"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 06:45"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:40"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:50"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:30"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:30"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 18:10")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:15"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }

        //=== SCENARIO 9: Pim är på gymmet och hämtar, Elina rapporterar inte position
        [Test]
        public void SC9_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 05:35"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 06:45"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:30"), A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:15"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 10: Pim lämnar + dubbelrapport från jobbet, Elina hämtar
        [Test]
        public void SC10_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 05:50"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 06:15"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:25"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:15"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:36"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:05"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:35"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 16:10")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 18:05"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 11: Pim hämtar + dubbelrapport från jobbet, Elina lämnar
        [Test]
        public void SC11_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:40"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:50"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:36"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:30"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:30"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 18:10")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:15"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 12: Pim lämnar + lunch, Elina hämtar
        [Test]
        public void SC12_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 05:50"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 06:15"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:25"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:15"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:05"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:35"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 16:10")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 18:05"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 13: Pim hämtar + lunch, Elina lämnar
        [Test]
        public void SC13_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:40"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:50"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:30"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:30"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 18:10")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:15"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 14: Pim lämnar + lunch, Elina ingen rapport
        [Test]
        public void SC14_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:25"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:05"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:35"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 18:05"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 15: Pim hämtar + lunch, Elina ingen rapport
        [Test]
        public void SC15_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("15:35"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:15"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 16: Pim hämtar + lunch + dubbelrapport för GPS+Wi-Fi, Elina lämnar
        [Test]
        public void SC16_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:16"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:40"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:51"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:50"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:26"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:51"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:36"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:16"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:30"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:30"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 18:10")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:16"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 17: Pim hämtar, Elina lämnar + tränar
        [Test]
        public void SC17_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:40"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:50"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:30"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 19:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 20:50"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:30"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 20:50")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:15"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 18: Pim hämtar + lunch, Elina lämnar (utebliven rapport 1)
        [Test]
        public void SC18_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:40"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:50"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            //_sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:30"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:30"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 18:10")),
                    new FamilyState.Member(PIM, MemberState.OnWayHome, DT("2016-10-19 15:35"))
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 19: Pim hämtar + lunch, Elina lämnar (utebliven rapport 2)
        [Test]
        public void SC19_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:40"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:50"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            //_sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            //_sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            //_sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            //_sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:30"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:30"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 18:10")),
                    new FamilyState.Member(PIM, MemberState.AtWork, DT("2016-10-19 07:50")),
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 20: Pim hämtar + lunch, Elina lämnar (utebliven rapport 3)
        [Test]
        public void SC20_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            //_sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            //_sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:40"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            //_sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            //_sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:50"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:30"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("17:30"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtHome, DT("2016-10-19 18:10")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:15")),
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        //=== SCENARIO 21: Pim hämtar + lunch, Elina lämnar (utebliven rapport 3)
        [Test]
        public void SC21_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:40"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 07:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 08:50"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:25"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 11:50"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 15:35"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 16:15"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            //_sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 17:30"), ELINA, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            //_sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-19 18:10"), ELINA, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustNotHaveHappened();
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA, MemberState.AtWork, DT("2016-10-19 08:50")),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-19 16:15")),
                }
            });
        }


        //=== SCENARIO 22: Real world scenario 161026, Pim lämnar + lunch, Elina rapporterar inte
        [Test]
        public void RWS1_ProcessHotspotEvent_Scenario()
        {
            // ARRANGE
            _currentState = new FamilyState { Members = new List<FamilyState.Member> { new FamilyState.Member(ELINA), new FamilyState.Member(PIM) } };
            A.CallTo(() => _stateRepository.GetState(A<Guid>._)).Returns(_currentState);

            // ACT
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-26 07:36"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-26 07:37"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-26 08:23"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-26 08:27"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-26 11:31"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-26 11:54"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-26 16:53"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-26 16:53"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-26 17:48"), PIM, A.Dummy<string>(), HotspotType.Work, ActionType.Leave), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-26 18:27"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());
            _sut.ProcessHotspotEvent(new HotspotEventMessage(DT("2016-10-26 18:28"), PIM, A.Dummy<string>(), HotspotType.Home, ActionType.Enter), A.Fake<TextWriter>());

            // ASSERT
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>._, A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _slackMessageSender.SendToSlack(A<string>.That.Contains("16:53"), A<TextWriter>._)).MustHaveHappened(Repeated.Exactly.Once);
            _currentState.ShouldBeEquivalentTo(new FamilyState
            {
                Members = new List<FamilyState.Member>
                {
                    new FamilyState.Member(ELINA),
                    new FamilyState.Member(PIM, MemberState.AtHome, DT("2016-10-26 18:28")),
                }
            }, opt => opt.Excluding(s => s.EventsThatTriggeredSlackMessages));
        }


        // ReSharper disable once InconsistentNaming
        private static DateTimeOffset DT(string input)
        {
            return DateTime.Parse(input);
        }

        private static FamilyProfile CreateStridFamily()
        {
            return new FamilyProfile
            {
                Id = FAMILY_ID,
                Adults = new List<UserProfile>
                {
                    new UserProfile { Id = PIM, LatestTimeForSchoolDropOff = new TimeSpan(8, 0, 0), LatestTimeForSchoolPickup = new TimeSpan(16, 30, 0), EarliestTimeForSchoolPickup = new TimeSpan(14, 0, 0)},
                    new UserProfile { Id = ELINA, LatestTimeForSchoolDropOff = new TimeSpan(8, 15, 0), LatestTimeForSchoolPickup = new TimeSpan(16, 30, 0), EarliestTimeForSchoolPickup = new TimeSpan(14, 0, 0)}
                }
            };
        }
    }
}
