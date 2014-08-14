Feature: Scheduling Reminders
	In order to manage a time-sensitive process
	As a client program
	I want to be able to schedule a reminder

Scenario: Scheduling a reminder
	When I request a reminder to be scheduled
	Then the response should be 201
	And the response should contain the reminderId for my reminder 
	And my payload should be delivered when it is due

Scenario: Cancelling a reminder
	Given I have requested a reminder
	When I send a cancellation request for this reminder
	Then the response should be xxx
	And my payload should not be delivered when it is due