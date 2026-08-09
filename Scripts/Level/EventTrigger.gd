extends Area3D

class_name EventTrigger

func trigger(body: Node3D) ->  void:
	
	if (body.is_in_group("Player")):
		event()
	
# Function MUST be overridden in order for the EventTrigger to do
# something meaningful in the level. Inheriting scripts should only
# change this function, and not the trigger.
#
# |
# |
# V

func event() -> void:
	push_warning("Event trigger action isn't overridden")
