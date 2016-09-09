﻿using System;

namespace QuestionBot.Model {

    public interface IRecord {
        string Question { get; set; }
        string Answer { get; set; }
        int ID { get; set; }
        DateTime TimeAsked { get; set; }
        DateTime TimeAnswered { get; set; }
    }
}
