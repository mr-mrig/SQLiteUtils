


--------------------------  TrainingPlanNote --------------------------


select id
from TrainingPlanNote
order by id desc
limit 1

;


INSERT INTO TrainingPlanNote
(
    Body
)
SELECT 'My Message ' || Id
FROM TrainingPlan
WHERE Id > 5032297		-- Insert the id from the previous query here

;

UPDATE TrainingPlan
SET TrainingPlanNoteId = Id

;




--------------------------  TrainingPlanMessage --------------------------

select id
from TrainingPlanMessage
order by id desc
limit 1

;


INSERT INTO TrainingPlanMessage
(
    Body
)
SELECT 'My Message ' || Id
FROM TrainingPlanRelation
WHERE Id > 5032297		-- Insert the id from the previous query here


;




--------------------------  WorkingSetNote --------------------------

select id
from WorkingSetNote
order by id desc
limit 1


;


INSERT INTO WorkingSetNote
(
    Body
)
SELECT 'My WS Note ' || Id
FROM WorkingSet
WHERE Id > 5032297		-- Insert the id from the previous query here


;




--------------------------  WorkUnitTemplateNote --------------------------

select id
from WorkUnitTemplateNote
order by id desc
limit 1


;


INSERT INTO WorkUnitTemplateNote
(
    Body
)
SELECT 'My WU Note ' || Id
FROM WorkUnitTemplate
WHERE Id > 5032297		-- Insert the id from the previous query here


;






