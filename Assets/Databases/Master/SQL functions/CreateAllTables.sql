-- Delete all tables
DROP TABLE IF EXISTS `Conversations_GerdnesAberdeen`;
DROP TABLE IF EXISTS `Conversations_OwinThePardoner`;
DROP TABLE IF EXISTS `Conversations_HamenTheInnkeeper`;
DROP TABLE IF EXISTS `Conversations_OenyusTheDrunk`;
DROP TABLE IF EXISTS `Conversations_RohezAberdeen`;

DROP TABLE IF EXISTS `Game_Characters`;
DROP TABLE IF EXISTS `Game_Events`;

-- Creating Converation tables
CREATE TABLE IF NOT EXISTS `Conversations_GerdnesAberdeen` ( `StateName` TEXT NOT NULL UNIQUE, `AddStates` TEXT NOT NULL, `CurrentSpeaker` TEXT NOT NULL, `Dialogue` TEXT NOT NULL, `Action` TEXT, `AddEvents` TEXT, `RemoveEvents` TEXT, `RequireEvents` TEXT, PRIMARY KEY(`StateName`), FOREIGN KEY(`CurrentSpeaker`) REFERENCES `Game_Characters`(`Name`), FOREIGN KEY(`AddEvents`) REFERENCES `Game_Events`(`Name`), FOREIGN KEY(`RemoveEvents`) REFERENCES `Game_Events`(`Name`), FOREIGN KEY(`RequireEvents`) REFERENCES Game_Events(Name) );
CREATE TABLE IF NOT EXISTS `Conversations_OwinThePardoner` ( `StateName` TEXT NOT NULL UNIQUE, `AddStates` TEXT NOT NULL, `CurrentSpeaker` TEXT NOT NULL, `Dialogue` TEXT NOT NULL, `Action` TEXT, `AddEvents` TEXT, `RemoveEvents` TEXT, `RequireEvents` TEXT, PRIMARY KEY(`StateName`), FOREIGN KEY(`CurrentSpeaker`) REFERENCES `Game_Characters`(`Name`), FOREIGN KEY(`AddEvents`) REFERENCES `Game_Events`(`Name`), FOREIGN KEY(`RemoveEvents`) REFERENCES `Game_Events`(`Name`), FOREIGN KEY(`RequireEvents`) REFERENCES Game_Events(Name) );
CREATE TABLE IF NOT EXISTS `Conversations_HamenTheInnkeeper` ( `StateName` TEXT NOT NULL UNIQUE, `AddStates` TEXT NOT NULL, `CurrentSpeaker` TEXT NOT NULL, `Dialogue` TEXT NOT NULL, `Action` TEXT, `AddEvents` TEXT, `RemoveEvents` TEXT, `RequireEvents` TEXT, PRIMARY KEY(`StateName`), FOREIGN KEY(`CurrentSpeaker`) REFERENCES `Game_Characters`(`Name`), FOREIGN KEY(`AddEvents`) REFERENCES `Game_Events`(`Name`), FOREIGN KEY(`RemoveEvents`) REFERENCES `Game_Events`(`Name`), FOREIGN KEY(`RequireEvents`) REFERENCES Game_Events(Name) );
CREATE TABLE IF NOT EXISTS `Conversations_OenyusTheDrunk` ( `StateName` TEXT NOT NULL UNIQUE, `AddStates` TEXT NOT NULL, `CurrentSpeaker` TEXT NOT NULL, `Dialogue` TEXT NOT NULL, `Action` TEXT, `AddEvents` TEXT, `RemoveEvents` TEXT, `RequireEvents` TEXT, PRIMARY KEY(`StateName`), FOREIGN KEY(`CurrentSpeaker`) REFERENCES `Game_Characters`(`Name`), FOREIGN KEY(`AddEvents`) REFERENCES `Game_Events`(`Name`), FOREIGN KEY(`RemoveEvents`) REFERENCES `Game_Events`(`Name`), FOREIGN KEY(`RequireEvents`) REFERENCES Game_Events(Name) );
CREATE TABLE IF NOT EXISTS `Conversations_RohezAberdeen` ( `StateName` TEXT NOT NULL UNIQUE, `AddStates` TEXT NOT NULL, `CurrentSpeaker` TEXT NOT NULL, `Dialogue` TEXT NOT NULL, `Action` TEXT, `AddEvents` TEXT, `RemoveEvents` TEXT, `RequireEvents` TEXT, PRIMARY KEY(`StateName`), FOREIGN KEY(`CurrentSpeaker`) REFERENCES `Game_Characters`(`Name`), FOREIGN KEY(`AddEvents`) REFERENCES `Game_Events`(`Name`), FOREIGN KEY(`RemoveEvents`) REFERENCES `Game_Events`(`Name`), FOREIGN KEY(`RequireEvents`) REFERENCES Game_Events(Name) );

-- Create Character tables
CREATE TABLE IF NOT EXISTS `Game_Characters` ( `Name` TEXT NOT NULL UNIQUE, `Description` TEXT NOT NULL, `Alias` TEXT, PRIMARY KEY(`Name`) );

-- Create Event tables
CREATE TABLE IF NOT EXISTS `Game_Events` ( `Name` TEXT NOT NULL UNIQUE, `Description` TEXT, `Status` INTEGER NOT NULL DEFAULT 0 CHECK(Status = 0 OR Status = 1), PRIMARY KEY(`Name`) );