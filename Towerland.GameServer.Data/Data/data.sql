CREATE DATABASE towerland;

CREATE USER IF NOT EXISTS 'root'@'%' IDENTIFIED BY '123456';
GRANT SELECT ON towerland.* TO 'root'@'%' IDENTIFIED BY '123456';
GRANT EXECUTE ON towerland.* TO 'root'@'%' IDENTIFIED BY '123456';
GRANT CREATE ON towerland.* TO 'root'@'%' IDENTIFIED BY '123456';
GRANT INSERT ON towerland.* TO 'root'@'%' IDENTIFIED BY '123456';
GRANT UPDATE ON towerland.* TO 'root'@'%' IDENTIFIED BY '123456';
GRANT DELETE ON towerland.* TO 'root'@'%' IDENTIFIED BY '123456';

USE towerland;

CREATE TABLE User(
  Id CHAR(36) PRIMARY KEY,
  FullName VARCHAR(128) NOT NULL,
  Email VARCHAR(128) NOT NULL,
  Nickname VARCHAR(128) NOT NULL,
  Password VARCHAR(128) NOT NULL,
  Experience INT DEFAULT 0
) Engine InnoDB CHARSET=utf8;

CREATE TABLE Battle(
  Id CHAR(36) PRIMARY KEY,
  Monsters_UserId CHAR(36) NOT NULL,
  Towers_UserId CHAR(36) NOT NULL,
  WinnerId CHAR(36),
  StartTime DATETIME,
  EndTime DATETIME
) Engine InnoDB CHARSET=utf8;

CREATE TABLE LiveBattleState(
  BattleId CHAR(36) PRIMARY KEY,
  Version INT NOT NULL,
  SerializedState JSON,
  SerializedActions JSON,
  FOREIGN KEY (BattleId) REFERENCES Battle(Id)
) Engine InnoDB CHARSET=utf8;

CREATE TABLE GameObject(
  Id INT PRIMARY KEY,
  Name VARCHAR(128) NOT NULL 
) Engine InnoDB CHARSET=utf8;

CREATE TABLE MonsterStats(
  GameObjectId INT PRIMARY KEY,
  Damage INT NOT NULL,
  Health INT NOT NULL,
  Speed INT NOT NULL,
  MovementPriority INT NOT NULL,
  IsAir TINYINT(1) NOT NULL,
  Cost INT NOT NULL,
  Defence INT NOT NULL,
  SpecialEffectId INT,
  SpecialEffectDuration INT,
  FOREIGN KEY (GameObjectId) REFERENCES GameObject(Id)
) Engine InnoDB CHARSET=utf8;

CREATE TABLE TowerStats(
  GameObjectId INT PRIMARY KEY,
  Damage INT NOT NULL,
  `Range` INT NOT NULL,
  AttackSpeed INT NOT NULL,
  TargetPriority INT NOT NULL,
  Cost INT NOT NULL,
  Attack INT NOT NULL,
  SpecialEffectId INT,
  SpecialEffectDuration INT,
  FOREIGN KEY (GameObjectId) REFERENCES GameObject(Id)
) Engine InnoDB CHARSET=utf8;