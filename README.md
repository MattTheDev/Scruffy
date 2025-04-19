# Scruffy

!! README WIP !!

## What is Scruffy?
Needed a way to purge messages on a schedule.
Wanted to be able to configure multiple channels based on purge interval in minutes.

## How to Scruffy?
`/ping` - Test bot response
`/channel channel:NameOfChannel purge-interval:IntervalInMinutes` - IntervalInMinutes must be between 5 and 10080

## How does it work?
Once configured, the bot checks cached messages within a channel. Once a message breaches the purge interval, it will remove the message. 

## Where To Find Us

![Discord](https://img.shields.io/discord/263688866978988032)

## Scruffy Status

[![Deploy Scruffy](https://github.com/MattTheDev/Scruffy/actions/workflows/deploy-scruffy.yml/badge.svg)](https://github.com/MattTheDev/Scruffy/actions/workflows/deploy-scruffy.yml) 
