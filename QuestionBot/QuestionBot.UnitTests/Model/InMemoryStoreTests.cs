﻿using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using QuestionBot.Model;

namespace QuestionBot.UnitTests.Model {

    [TestFixture]
    public class InMemoryStoreTests{
        private IStore _storeTest;
        private IEnumerable<IRecord> _retrievedRecords;

        [SetUp]
        public void Setup(){
            _storeTest = new InMemoryStore();
        }

        [Test]
        public void New_question_creates_record(){
            string question = "What is 1+2?";

            IRecord questionRecord = _storeTest.CreateRecord(question);

            Assert.AreEqual(questionRecord.Question, question);
            Assert.AreNotEqual(questionRecord.TimeAsked, DateTime.MinValue);
            Assert.AreNotEqual(questionRecord.ID,0);
        }

        [Test]
        public void Updating_question_with_answer_stores_my_answer(){
            string question = "What is 1+2?";
            string answer = "3";

            IRecord questionRecord = _storeTest.CreateRecord(question);
            bool updatedRecord = _storeTest.TryUpdateRecord(questionRecord.ID, answer);
            _retrievedRecords = _storeTest.GetRecords();
            IRecord myRecord = _retrievedRecords.ElementAt(0);

            Assert.IsTrue(updatedRecord);
            Assert.AreEqual(myRecord.Question, question);
            Assert.AreEqual(myRecord.Answer, answer);
            Assert.AreEqual(myRecord.TimeAsked, questionRecord.TimeAsked);
            Assert.AreEqual(myRecord.TimeAnswered, questionRecord.TimeAnswered);
            Assert.AreEqual(myRecord.ID, questionRecord.ID);
        }

        [Test]
        public void GetRecords_returns_all_question_records(){
            string question1 = "What is 1+2";
            string question2 = "What is 2+3?";
            string answer1 = "3";
            string answer2 = "five";
           
            IRecord record1 = _storeTest.CreateRecord(question1);
            IRecord record2 = _storeTest.CreateRecord(question2);
            IEnumerable<IRecord> questionRecords = new List<IRecord>{record1,record2};

            _storeTest.TryUpdateRecord(record1.ID, answer1);
            _storeTest.TryUpdateRecord(record2.ID, answer2);

            _retrievedRecords = _storeTest.GetRecords();

            CollectionAssert.AreEqual(questionRecords, _retrievedRecords);
        }

        [Test]
        public void GetRecords_returns_empty_list_if_there_are_no_records(){
            _retrievedRecords = _storeTest.GetRecords();

            Assert.IsEmpty(_retrievedRecords);
        }

        [Test]
        public void Updating_record_with_blank_answer_returns_false(){
            string question1 = "What is 1+2";
            string question2 = "What is 2+3?";
            string answer1 = "";
            string answer2 = null;

            IRecord record1 = _storeTest.CreateRecord(question1);
            IRecord record2 = _storeTest.CreateRecord(question2);

            bool emptyAnswerResult = _storeTest.TryUpdateRecord(record1.ID, answer1);
            bool nullAnswerResult = _storeTest.TryUpdateRecord(record2.ID, answer2);

            Assert.IsFalse(emptyAnswerResult);
            Assert.IsFalse(nullAnswerResult);
        }

        [Test]
        public void Updating_record_with_nonexistant_ID_returns_false(){
            string question1 = "What is 1+2";
            string answer = "3";

            IRecord myRecord = _storeTest.CreateRecord(question1);

            bool badIDResult = _storeTest.TryUpdateRecord(myRecord.ID + 1, answer);

            Assert.IsFalse(badIDResult);
        }
    }
}
