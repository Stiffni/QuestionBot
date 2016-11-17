﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using QuestionBot.Model;
using Moq;

namespace QuestionBot.UnitTests.Model {
    [TestFixture]
    internal class MessageEmitterTests {
        private MessageEmitter _testEmitter;
        private Mock<IMessageListener> _testListener;
        private Mock<IConsole> _testConsole;

        [SetUp]
        public void Setup() {
            _testListener = new Mock<IMessageListener>( MockBehavior.Strict );
            _testConsole = new Mock<IConsole>();
        }

        [Test]
        public void Receiving_Message_Will_Notify_Listener() {
            const string message = "/question What is 1+1?";
            const string output = "Thank you";
            const string nullOutput = null;
            const string exitCommand = "/exitQuestionBot";

            _testConsole.SetupSequence( x => x.ReadLine() )
                .Returns( message )
                .Returns( "/exitQuestionBot" );

            _testListener.Setup( x => x.ReceiveMessage( message ) ).Returns( output );
            _testListener.Setup( x => x.ReceiveMessage( exitCommand ) ).Returns( nullOutput );

            _testEmitter = new MessageEmitter( _testConsole.Object );
            _testEmitter.Add( _testListener.Object );
            _testEmitter.Start();

            _testListener.Verify( x => x.ReceiveMessage( message ), Times.Exactly( 1 ) );
            _testListener.Verify( x => x.ReceiveMessage( exitCommand ), Times.Exactly( 1 ) );
            _testConsole.Verify( x => x.WriteLine( output ), Times.Exactly( 1 ) );
        }

        [Test]
        public void Receiving_Message_With_No_Listeners_Will_Not_Call_ReceiveMessage() {
            const string message = "/question What is 1+1?";
            const string output = "Thank you";

            _testConsole.SetupSequence( x => x.ReadLine() )
                .Returns( message )
                .Returns( "/exitQuestionBot" );

            _testListener.Setup( x => x.ReceiveMessage( message ) ).Returns( output );

            _testEmitter = new MessageEmitter( _testConsole.Object );
            _testEmitter.Start();

            _testListener.Verify( x => x.ReceiveMessage( message ), Times.Never );
        }

        [Test]
        public void Adding_Null_Listener_Will_Not_Throw() {
            const string message = "/question What is 1+1?";

            _testConsole.Setup( x => x.ReadLine() ).Returns( message );
            _testEmitter = new MessageEmitter( _testConsole.Object );

            Assert.DoesNotThrow( () => _testEmitter.Add( null ) );
        }
    }
}