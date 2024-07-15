# SimpleChat

## To run the application:

1. Run docker compose
2. Build Database. Open Package Manager Console and run the following commands:
update-database

## Examples of commands to interact with chats (Postman)
- Setup client
{
  "protocol": "json",
  "version": 1
}

- Send message:
first argument: chatId | second argument: userId, | third argument: message
{
    "arguments": ["22557CE6-A8AC-434A-8DD2-046544637ACC", "0A147B0B-3489-4C0B-8DD6-C6628890E506", "Hello"],
    "target": "SendMessage",
    "type": 1
}

- Join chat
first argument: chatId | second argument: userId
{
   "arguments": ["22557CE6-A8AC-434A-8DD2-046544637ACC", "0A147B0B-3489-4C0B-8DD6-C6628890E506"],
   "target": "JoinChat",
   "type": 1
}

- Leave chat
first argument: chatId | second argument: userId
{
   "arguments": ["375BA016-2F5D-454C-8DD8-492C9BC306A9", "EA927493-C250-4881-8696-61623AEBEFB9"],
   "target": "LeaveChat",
   "type": 1
}

Enabled swagger support for API calls.
