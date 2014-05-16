ForgetMeNot
===========

Reminder / timeout service to aid coordintation of workflow in a distributed system


Request a reminder:
-------------------

Headers:
  - Confirmation URL - calls this URL with the ID (handle) of your reminder
  - Delivery URL - on time-out, this is where the payload will be delivered
  - SendCancellations - true / false - do you want to have cancelled reminders delivered anyway (can't think of a good reason why)
  - RemindMeAt (time in the future. Relative or Absolute?)

Body:
  - the payload to deliver at time-out


Cancellation:
-------------

Headers:

Body:
  - ReminderId (UUID / GUID?)


Architecture:
-------------

HTTP interface for making reminder requests and cancellation requests. 
Transports for RabbitMQ look like a concrete requirement at this stage.

Priority Queue - contains the times of all time-outs. The priority function is simply sorting items by time, so the most recent items can be popped off the front of the queue. Backed by a linked-list for O(log n) complexity, since we will be doing plenty of inserts.

Backed by one of binary heap, linked-list, binary-search-tree.

Deletion - couple of ideas:
  - when cancelling a timeout we can remove from the priority queue
OR
  - we can keep another list of cancelled timeouts(revocation lilst). When a timeout occurs we can check against the revocation list. If the item is in the list, we do not send the payload AND we remove the item from the list. ELSE we send the payload.

Prefer 2 because it means that we dont have to remove items from the file. If we keep 1 then we are only ever appending to the file -> forward only, immutable. Can get leverage from GES transaction log for this?

