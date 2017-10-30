﻿/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using JetBrains.Threading;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.Commons.Generators;
using KaVE.VS.Commons.TestUtils.Generators;
using NUnit.Framework;

namespace KaVE.VS.Commons.Tests.Generators
{
    // do not extend! this class is only used to test basic functionality of the base test
    internal sealed class EventGeneratorTestBaseTest : EventGeneratorTestBase
    {
        private TestEventGenerator _uut;

        private class TestEventGenerator : EventGeneratorBase
        {
            public TestEventGenerator([NotNull] IRSEnv env,
                [NotNull] IMessageBus messageBus,
                [NotNull] IDateUtils dateUtils,
                [NotNull] IThreading threading) : base(env, messageBus, dateUtils, threading) { }

            public void FireTestIDEEvent()
            {
                Fire(Create<TestIDEEvent>());
            }

            public void FireTestIDEEventNow()
            {
                FireNow(Create<TestIDEEvent>());
            }
        }

        [SetUp]
        public void BaseTestSetUp()
        {
            _uut = new TestEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils, TestThreading);
        }

        [Test]
        public void ShouldSetExtensionVersion()
        {
            TestRSEnv.KaVEVersion = "1.0-test";

            _uut.FireTestIDEEvent();

            var ideEvent = GetSinglePublished<TestIDEEvent>();
            Assert.AreEqual("1.0-test", ideEvent.KaVEVersion);
        }

        [Test]
        public void ShouldSetDateToNow()
        {
            _uut.FireTestIDEEventNow();

            var ideEvent = GetSinglePublished<TestIDEEvent>();
            Assert.AreEqual(TestDateUtils.Now, ideEvent.TriggeredAt);
        }
    }
}