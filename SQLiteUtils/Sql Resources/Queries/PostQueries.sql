




-- POSTS_FINAL_0


-- Should be the final query for searching the Posts and their required info

-- PERFORMANCE: Never-ending query...

-- NOTE: This is the query for the first page (with latest 20 posts).
--		When the user scrolls down, other 20 posts must be fetched according to the last displayed timestamp -> This is done by decommenting the WHERE caluse on the Timestamp (see below)




SELECT 

CASE
WHEN F.Id IS NOT NULL THEN 'FitDay'
WHEN M.Id IS NOT NULL THEN 'MeasDay'
WHEN DP.Id IS NOT NULL THEN 'DietPlan'
ELSE ''
END AS PostType,


-- User and Post data here
U.Id as UserId, U.Username, P.Caption, Date(P.CreatedOn, 'unixepoch') as PostDate,

-- FitnessDayEntry data here
F.Id as FitId, W.Kg / 10.0 as WeightKg, 
(
    SELECT Abs(W.Kg - Kg) / 10.0
    FROM Weight
    JOIN FitnessDayEntry
    USING(Id)
    JOIN Post
    USING(Id)
    WHERE Post.UserId = P.UserId
    ORDER BY FitnessDayEntry.DayDate DESC
    LIMIT 1
) as WeightDiff,
4*(DD.CarbGrams + DD.ProteinGrams) + 9 * DD.FatGrams AS CalIntake,

-- DietPlan data here
DP.Id as DietPlanId, Dp.Name as DietPlanName,

--Measures data here
M.Id as MeasId, CC.*, BIA.Bf, PLI.Bf,

-- Image data here
I.Id, I.Url

-- User, Posts and Images
FROM Post P
JOIN User U
ON U.Id = P.UserId
LEFT JOIN Image I
ON P.Id = I.PostId

-- FitnessDayEntry
LEFT JOIN FitnessDayEntry F
ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

-- DietPlan
LEFT JOIN DietPlan DP
ON P.Id = DP.Id
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

-- Measures
LEFT JOIN MeasuresEntry M
ON P.Id = M.Id
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id

-- Likes, Comments
LEFT JOIN
(
    SELECT PostId, COUNT(Id) as CommentsCount, COALESCE(LastUpdate, COALESCE(MAX(CreatedOn), 0)) AS LastCommentTs
    FROM Comment C
    GROUP BY PostId
) CommentsTemp
ON P.Id = CommentsTemp.PostId
LEFT JOIN
(
    SELECT PostId, COUNT(UserId) AS LikesCount, COALESCE(MAX(CreatedOn), 0) AS LastLikeTs
    FROM UserLiked L
    GROUP BY PostId
) LikesTemp
ON P.Id = LikesTemp.PostId


WHERE P.UserId IN
(
    12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
    677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
    1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
    1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
    2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
    2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
    3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
    4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
    4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
    5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
    6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
    7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
    8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
    8685, 8698, 8745

)
AND P.IsPublic = true

--	AND MAX(P.CreatedOn, LastLikeTs, LastCommentTs) < <my timestamp>				-- Subsequent queries


ORDER BY MAX(P.CreatedOn, LastLikeTs, LastCommentTs) DESC

LIMIT 20;









;




-- POSTS_FINAL_1


-- Should be the final query for searching the Posts and their required info

--PERFORMANCE: way faster than POSTS_FINAL_0

-- NOTE: This is the query for the first page (with latest 20 posts).
--		When the user scrolls down, other 20 posts must be fetched according to the last displayed timestamp -> This is done by decommenting the WHERE caluse on the Timestamp (see below)

-- NOTE: This query fetches the followees posts only. It must include the UserId in an OR condition.
--       -> As the followees can be easily fetched from the local DB, this query is likely to be suboptimal. Look at POSTS_1


SELECT 

CASE
WHEN F.Id IS NOT NULL THEN 'FitDay'
WHEN M.Id IS NOT NULL THEN 'MeasDay'
WHEN DP.Id IS NOT NULL THEN 'DietPlan'
ELSE ''
END AS PostType,

-- Likes annd Comments data here
(    SELECT COUNT(UserId) AS LikesCount
    FROM UserLiked L
    WHERE PostId = P.Id
) as LikesCount,
(
    SELECT COUNT(Id) as CommentsCount
    FROM Comment C
    WHERE PostId = P.Id
) as CommentsCount,
(
    SELECT COALESCE(MAX(L.CreatedOn), 0)
    FROM UserLiked L
    WHERE PostId = P.Id
) as LastLikeTs,
(
    SELECT COALESCE(LastUpdate, COALESCE(MAX(CreatedOn), 0))
    FROM Comment C
    WHERE PostId = P.Id
) as LastCommentTs,

-- User and Post data here
U.Id as UserId, U.Username, P.Caption, Date(P.CreatedOn, 'unixepoch') as PostDate,

-- FitnessDayEntry data here
F.Id as FitId, W.Kg / 10.0 as WeightKg, 
(
    SELECT Abs(W.Kg - Kg) / 10.0
    FROM Weight
    JOIN FitnessDayEntry
    USING(Id)
    JOIN Post
    USING(Id)
    WHERE Post.UserId = P.UserId
    ORDER BY FitnessDayEntry.DayDate DESC
    LIMIT 1
) as WeightDiff,
4*(DD.CarbGrams + DD.ProteinGrams) + 9 * DD.FatGrams AS CalIntake,

-- DietPlan data here
DP.Id as DietPlanId, Dp.Name as DietPlanName,

--Measures data here
M.Id as MeasId, CC.*, BIA.Bf, PLI.Bf,

-- Image data here
I.Id, I.Url

-- User, Posts and Images
FROM Post P
JOIN User U
ON U.Id = P.UserId
LEFT JOIN Image I
ON P.Id = I.PostId

-- FitnessDayEntry
LEFT JOIN FitnessDayEntry F
ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

-- DietPlan
LEFT JOIN DietPlan DP
ON P.Id = DP.Id
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

-- Measures
LEFT JOIN MeasuresEntry M
ON P.Id = M.Id
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id



WHERE P.UserId IN
(
    12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
    677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
    1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
    1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
    2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
    2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
    3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
    4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
    4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
    5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
    6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
    7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
    8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
    8685, 8698, 8745

)
AND P.IsPublic = true

--	AND MAX(P.CreatedOn, LastLikeTs, LastCommentTs) < <my timestamp>				-- Subsequent queries

ORDER BY MAX(P.CreatedOn, LastLikeTs, LastCommentTs) DESC

LIMIT 20








;





-- POSTS_FINAL_2


-- POST_FINAL_1 -> Ordering by an indexed field instead of the aggregate function

--PERFORMANCE: 300x faster than POSTS_FINAL_1

-- NOTE: This is the query for the first page (with latest 20 posts).
--		When the user scrolls down, other 20 posts must be fetched according to the last displayed timestamp -> This is done by decommenting the WHERE caluse on the Timestamp (see below)

-- NOTE: This query doesn't sort for the last activity, but for the CreatedOn, which is indexed.
--       The huge performance increase telss that the best option is likely to be to sort for a P.LastUpdate field which must be indexed.
--		The real imporvement is due to the ORDER BY, not to the two less queries
--	 	The drawback is that each comment/like should TRIGGER an update on the Post table.
--		More evaluation is needed.....


SELECT 

CASE
WHEN F.Id IS NOT NULL THEN 'FitDay'
WHEN M.Id IS NOT NULL THEN 'MeasDay'
WHEN DP.Id IS NOT NULL THEN 'DietPlan'
ELSE ''
END AS PostType,

-- Likes annd Comments data here
(    SELECT COUNT(UserId) AS LikesCount
    FROM UserLiked L
    WHERE PostId = P.Id
) as LikesCount,
(
    SELECT COUNT(Id) as CommentsCount
    FROM Comment C
    WHERE PostId = P.Id
) as CommentsCount,


-- User and Post data here
U.Id as UserId, U.Username, P.Caption, Date(P.CreatedOn, 'unixepoch') as PostDate,

-- FitnessDayEntry data here
F.Id as FitId, W.Kg / 10.0 as WeightKg, 
(
    SELECT Abs(W.Kg - Kg) / 10.0
    FROM Weight
    JOIN FitnessDayEntry
    USING(Id)
    JOIN Post
    USING(Id)
    WHERE Post.UserId = P.UserId
    ORDER BY FitnessDayEntry.DayDate DESC
    LIMIT 1
) as WeightDiff,
4*(DD.CarbGrams + DD.ProteinGrams) + 9 * DD.FatGrams AS CalIntake,

-- DietPlan data here
DP.Id as DietPlanId, Dp.Name as DietPlanName,

--Measures data here
M.Id as MeasId, CC.*, BIA.Bf, PLI.Bf,

-- Image data here
I.Id, I.Url

-- User, Posts and Images
FROM Post P
JOIN User U
ON U.Id = P.UserId
LEFT JOIN Image I
ON P.Id = I.PostId

-- FitnessDayEntry
LEFT JOIN FitnessDayEntry F
ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

-- DietPlan
LEFT JOIN DietPlan DP
ON P.Id = DP.Id
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

-- Measures
LEFT JOIN MeasuresEntry M
ON P.Id = M.Id
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id



WHERE P.UserId IN
(
    12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
    677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
    1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
    1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
    2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
    2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
    3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
    4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
    4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
    5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
    6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
    7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
    8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
    8685, 8698, 8745

)
AND P.IsPublic = true

--	AND P.CreatedOn < <my timestamp>				-- Subsequent queries

ORDER BY P.CreatedOn DESC

LIMIT 20








;





-- POST_COMMENTS_LIKES_0


-- Get the Comments and likes for the selected Post. To be used when the user selects a Post on its timeline.


-- NOTE: This query fetches the followees posts only. It must include the UserId in an OR condition.
--       -> As the followees can be easily fetched from the local DB, this query is likely to be suboptimal. Look at POSTS_1


SELECT * 
FROM
(
select UserId, Username, Body, 'Comment'
FROM Comment
LEFT JOIN User
ON Comment.UserId = User.Id
WHERE PostId = 13332470
ORDER BY CreatedOn
)
UNION ALL

Select UserId, Username, Value, 'Like'
FROM UserLiked
LEFT JOIN User
ON UserLiked.UserId = User.Id
WHERE PostId = 13332470






;




-- POSTS_0


-- Get the Posts of the user and all his followees


-- NOTE: This query fetches the followees posts only. It must include the UserId in an OR condition.
--       -> As the followees can be easily fetched from the local DB, this query is likely to be suboptimal. Look at POSTS_1



SELECT

CASE
WHEN F.Id IS NOT NULL THEN 'FitDay'
WHEN M.Id IS NOT NULL THEN 'MeasDay'
WHEN DP.Id IS NOT NULL THEN 'DietPlan'
ELSE ''
END AS PostType,

P.UserId, P.Id as PostId, F.Id, M.Id
, DP.Id, P.Caption, C.Body, L.Value as Liked
, *		-- To be removed

FROM User U
JOIN UserRelation UR
ON U.Id = UR.SourceUserId
JOIN User U1
ON U1.Id = UR.TargetUserId

-- Posts and Images
JOIN Post P
ON P.UserId = U1.Id
LEFT JOIN Image I
ON P.Id = I.PostId


-- FitnessDayEntry
LEFT JOIN FitnessDayEntry F
ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

-- DietPlan
LEFT JOIN DietPlan DP
ON P.Id = DP.Id
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

-- Measures
LEFT JOIN MeasuresEntry M
ON P.Id = M.Id
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id

-- Comments, Likes
LEFT JOIN Comment C
ON P.Id = C.PostId
LEFT JOIN UserLiked L
ON P.Id = L.PostId

WHERE U.Id = 12
AND UR.RelationStatusId = 1


ORDER BY P.CreatedOn DESC

LIMIT 100000       -- 100K



;






-- POSTS_1


-- Get the Posts of the user and all his followees
-- USER_LIST


-- PERFORMANCE: 7x faster than POSTS_0

-- NOTE: This query requires the followees list to be pre-feched. The list must include the UserId as well



SELECT

CASE
WHEN F.Id IS NOT NULL THEN 'FitDay'
WHEN M.Id IS NOT NULL THEN 'MeasDay'
WHEN DP.Id IS NOT NULL THEN 'DietPlan'
ELSE ''
END AS PostType,

P.UserId, P.Id as PostId, F.Id, M.Id
, DP.Id, P.Caption, C.Body, L.Value as Liked
,*		-- To be removed

-- Posts and Images
FROM Post P
LEFT JOIN Image I
ON P.Id = I.PostId

-- FitnessDayEntry
LEFT JOIN FitnessDayEntry F
ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

-- DietPlan
LEFT JOIN DietPlan DP
ON P.Id = DP.Id
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

-- Measures
LEFT JOIN MeasuresEntry M
ON P.Id = M.Id
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id

-- Comments, Likes
LEFT JOIN Comment C
ON P.Id = C.PostId
LEFT JOIN UserLiked L
ON P.Id = L.PostId

WHERE P.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)


ORDER BY P.CreatedOn DESC

LIMIT 100000       -- 100K


;






-- POSTS_2


-- Get the Posts of the user and all his followees
-- USER_LIST


-- PERFORMANCE: Slower than POSTS_0

-- NOTE: 



SELECT

CASE
WHEN FTemp.TempId IS NOT NULL THEN 'FitDay'
WHEN M.Id IS NOT NULL THEN 'MeasDay'
WHEN DP.Id IS NOT NULL THEN 'DietPlan'
ELSE ''
END AS PostType,

P.UserId, P.Id as PostId, FTemp.TempId, M.Id
, DP.Id, P.Caption, C.Body, L.Value as Liked
, *		-- To be removed

-- Posts and Images
FROM Post P
LEFT JOIN Image I
ON P.Id = I.PostId

-- FitnessDayEntry
LEFT JOIN
(
SELECT P.Id as TempId, *

FROM Post P
JOIN FitnessDayEntry F
ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

WHERE P.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745
)
) FTemp
ON FTemp.TempId = P.Id

-- DietPlan
LEFT JOIN DietPlan DP
ON P.Id = DP.Id
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

-- Measures
LEFT JOIN MeasuresEntry M
ON P.Id = M.Id
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id

-- Comments, Likes
LEFT JOIN Comment C
ON P.Id = C.PostId
LEFT JOIN UserLiked L
ON P.Id = L.PostId

WHERE P.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)


ORDER BY P.CreatedOn DESC

LIMIT 100000       -- 100K






;




-- POSTS_3


-- Denormalized tables
-- USER_LIST


-- PERFORMANCE: Slowest one

-- NOTE: No needs for subquiires since the query is not joining the Post table more than once.
--		--> No need for further table filtering



SELECT

CASE
WHEN FTemp.TempId IS NOT NULL THEN 'FitDay'
WHEN MTemp.TempId IS NOT NULL THEN 'MeasDay'
WHEN DTemp.TempId IS NOT NULL THEN 'DietPlan'
ELSE ''
END AS PostType,

P.UserId, P.Id as PostId, FTemp.TempId, MTemp.TempId
, DTemp.TempId, P.Caption, C.Body, L.Value as Liked
, *


-- Posts and Images
FROM Post P
LEFT JOIN Image I
ON P.Id = I.PostId

-- FitnessDayEntry
LEFT JOIN
(
SELECT F.Id as TempId, *

--FROM Post P
--JOIN FitnessDayEntry F
FROM FitnessDayEntry F
--ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

WHERE F.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745
)
) FTemp
ON FTemp.TempId = P.Id

-- DietPlan
LEFT JOIN
(
SELECT DP.Id as TempId, *

FROM DietPlan DP
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

WHERE DP.OwnerId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745
)
) DTemp
ON DTemp.TempId = P.Id


-- Measures
LEFT JOIN
(
SELECT M.Id as TempId, *

FROM MeasuresEntry M
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id

WHERE M.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745
)
) MTemp
ON MTemp.TempId = P.Id

-- Comments, Likes
LEFT JOIN Comment C
ON P.Id = C.PostId
LEFT JOIN UserLiked L
ON P.Id = L.PostId

WHERE P.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)


ORDER BY P.CreatedOn DESC

LIMIT 100000       -- 100K





;





;






-- POSTS_4 


-- Get the Posts of the user and all his followees. 
-- USER_LIST


-- PERFORMANCE: 2x faster than POSTS_2, just by fetching less fields
--				2x faster than POSTS_FINAL_2, with the plus that it already fetches comments and likes and the drawback that the COUNT must be post-processed.
--				The problem with these kind of queries is that they cannot LIMIT the number of posts but the number of Join (Post, Comments, Likes) -> If there's a post with a lot of comments, many queries must be repeated to fill the timeline!





SELECT

CASE
WHEN F.Id IS NOT NULL THEN 'FitDay'
WHEN M.Id IS NOT NULL THEN 'MeasDay'
WHEN DP.Id IS NOT NULL THEN 'DietPlan'
ELSE ''
END AS PostType,

P.UserId, P.Id as PostId, F.Id, M.Id
, DP.Id, P.Caption, C.Body, L.Value as Liked
-- Fetch one data from all the tables so the other tables are included in the query
, DD.CarbGrams, DDT.Name, W.Kg / 10 as WeightKg, A.Steps, WD.Glycemia,
DPU.StartDate, DPD.Name,
CC.Waist, BIA.Bf, PLI.Bf



-- Posts and Images
FROM Post P
LEFT JOIN Image I
ON P.Id = I.PostId

-- FitnessDayEntry
LEFT JOIN FitnessDayEntry F
ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

-- DietPlan
LEFT JOIN DietPlan DP
ON P.Id = DP.Id
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

-- Measures
LEFT JOIN MeasuresEntry M
ON P.Id = M.Id
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id

-- Comments, Likes
LEFT JOIN Comment C
ON P.Id = C.PostId
LEFT JOIN UserLiked L
ON P.Id = L.PostId

WHERE P.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)


ORDER BY P.CreatedOn DESC

LIMIT 100000 -- 100K







;








-- POSTS_5


-- Get the Posts of the user and all his followees. This is njust a dummy query to test timings, as it misses the simple Posts. The final results will be slower
-- USER_LIST + UNION ALL


-- PERFORMANCE: 

-- NOTE: 





--EXPLAIN QUERY PLAN


SELECT *
FROM
(
-- FitnessDayEntry
SELECT 'FitDay' AS PostType
, F.DayDate as CreatedOn, F.Id, DD.CarbGrams, DDT.Name, W.Kg / 10 as WeightKg, A.Steps, WD.Glycemia

--FROM Post P
--JOIN FitnessDayEntry F
FROM FitnessDayEntry F
--ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

WHERE F.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)


UNION ALL


-- Measures
SELECT 'MeasDay' AS PostType
, M.MeasureDate as CreatedOn, M.Id, CC.Waist, BIA.Bf as BiaBf, PLI.Bf as PliBf, '', ''


--FROM Post P
--JOIN MeasuresEntry M
--ON P.Id = M.Id
FROM MeasuresEntry M
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id

WHERE M.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)


UNION ALL


-- DietPlan
SELECT 'DietPlan' AS PostType, DP.Id
, DP.CreatedOn as CreatedOn,  DPD.CarbGrams, DPU.StartDate, DDT2.Name, '', ''

--FROM Post P
--JOIN DietPlan DP
--ON P.Id = DP.Id
FROM DietPlan DP
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

WHERE DP.OwnerId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)

)

ORDER BY CreatedOn DESC






;




-- POSTS_6


-- Get the Posts of the user and all his followees.
-- USER_LIST + UNION ALL


-- PERFORMANCE: 2x faster than POSTS_4

-- NOTE: 


SELECT *
FROM
(
-- FitnessDayEntry
SELECT 'FitDay' AS PostType
, F.DayDate as CreatedOn, F.Id, DD.CarbGrams, DDT.Name, W.Kg / 10 as WeightKg, A.Steps, WD.Glycemia

--FROM Post P
--JOIN FitnessDayEntry F
FROM FitnessDayEntry F
--ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

WHERE F.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)


UNION ALL


-- Measures
SELECT 'MeasDay' AS PostType
, M.MeasureDate as CreatedOn, M.Id, CC.Waist, BIA.Bf as BiaBf, PLI.Bf as PliBf, '', ''


--FROM Post P
--JOIN MeasuresEntry M
--ON P.Id = M.Id
FROM MeasuresEntry M
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id

WHERE M.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)


UNION ALL


-- DietPlan
SELECT 'DietPlan' AS PostType, DP.Id
, DP.CreatedOn as CreatedOn,  DPD.CarbGrams, DPU.StartDate, DDT2.Name, '', ''

--FROM Post P
--JOIN DietPlan DP
--ON P.Id = DP.Id
FROM DietPlan DP
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

WHERE DP.OwnerId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)

)

ORDER BY CreatedOn DESC

LIMIT 100000








;







-- POSTS_COUNT_0


-- Get the Posts of the user and all his followees, fetching the number of likes and comments
-- COUNT(LIKES,COMMENTS)


-- PERFORMANCE: 30% Slower than POSTS_2

-- NOTE: Doesn't fetche the comments and likes, which must be obtained via PostId in a subsequent query. 
--		Probably it's the best solution: user doesn't need to fetch all the comments of all the posts but only the ones he selects.
--		The alternative is to use POSTS_2 for fetching everything, then post-process via C# for counting the comments and likes


SELECT

CASE
WHEN F.Id IS NOT NULL THEN 'FitDay'
WHEN M.Id IS NOT NULL THEN 'MeasDay'
WHEN DP.Id IS NOT NULL THEN 'DietPlan'
ELSE ''
END AS PostType,

P.UserId, P.Id as PostId, F.Id, M.Id
, DP.Id, P.Caption
,*
,(
SELECT COUNT(UserId)
FROM UserLiked L
WHERE PostId = P.Id

) as LikesCount
,(
SELECT COUNT(Id)
FROM Comment C
WHERE PostId = P.Id

) as CommentsCount

-- Posts and Images
FROM Post P
LEFT JOIN Image I
ON P.Id = I.PostId

-- FitnessDayEntry
LEFT JOIN FitnessDayEntry F
ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

-- DietPlan
LEFT JOIN DietPlan DP
ON P.Id = DP.Id
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

-- Measures
LEFT JOIN MeasuresEntry M
ON P.Id = M.Id
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id


-- No join with Comments and likes

WHERE P.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)







;


-- POSTS_COUNT_1


-- Get the Posts of the user and all his followees, fetching the number of likes and comments
-- COUNT(LIKES,COMMENTS)


-- PERFORMANCE: 400x slower than POSTS_COUNT_0

-- NOTE: 



WITH CommentsCnt AS
(
SELECT PostId, count(1) CommentsCount
FROM Comment
GROUP BY PostId
),
LikesCnt AS
(
SELECT PostId, count(1) LikesCount
FROM UserLiked
GROUP BY PostId
)

SELECT

CASE
WHEN F.Id IS NOT NULL THEN 'FitDay'
WHEN M.Id IS NOT NULL THEN 'MeasDay'
WHEN DP.Id IS NOT NULL THEN 'DietPlan'
ELSE ''
END AS PostType,

P.UserId, P.Id as PostId, F.Id, M.Id
, DP.Id, P.Caption
,*
, CommentsCount, LikesCount


-- Posts and Images
FROM Post P
LEFT JOIN Image I
ON P.Id = I.PostId

-- FitnessDayEntry
LEFT JOIN FitnessDayEntry F
ON P.Id = F.Id
LEFT JOIN DietDay DD
ON F.Id = DD.Id
LEFT JOIN DietDayType DDT
ON DD.DietDayTypeId = DDT.Id
LEFT JOIN Weight W
ON F.Id = W.Id
LEFT JOIN ActivityDay A
ON F.Id = A.Id
LEFT JOIN WellnessDay WD
ON F.Id = WD.Id

-- DietPlan
LEFT JOIN DietPlan DP
ON P.Id = DP.Id
LEFT JOIN DietPlanUnit DPU
ON DP.Id = DPU.DietPlanId
LEFT JOIN DietPlanDay DPD
ON DPU.Id = DPD.DietPlanUnitId
LEFT JOIN DietDayType DDT2
ON DPD.DietDayTypeId = DDT2.Id

-- Measures
LEFT JOIN MeasuresEntry M
ON P.Id = M.Id
LEFT JOIN Circumference CC
ON M.Id = CC.Id
LEFT JOIN BiaEntry BIA
ON M.Id = BIA.Id
LEFT JOIN Plicometry PLI
ON M.Id = PLI.Id

-- Comments, Likes
LEFT JOIN CommentsCnt CC
ON P.Id = CC.PostId
LEFT JOIN LikesCnt C
ON P.Id = C.PostId


WHERE P.UserId IN
(
12, 5, 27, 43, 102, 157, 176, 254, 291, 294, 304, 458, 576, 582, 639,
677, 752, 809, 857, 877, 878, 889, 935, 992, 1001, 1046, 1079, 1131, 1145,
1273, 1339, 1350, 1356, 1364, 1376, 1467, 1503, 1545, 1630, 1635, 1677, 1719,
1764, 1804, 1835, 1853, 1891, 1957, 2008, 2027, 2042, 2111, 2192, 2199, 2274,
2437, 2475, 2597, 2663, 2688, 2714, 2729, 2738, 2771, 2791, 2862, 2871, 2901,
2947, 3023, 3071, 3103, 3106, 3194, 3319, 3404, 3523, 3539, 3614, 3661, 3734,
3803, 3833, 3835, 3840, 3846, 3958, 3965, 4038, 4117, 4235, 4350, 4416, 4425,
4429, 4432, 4435, 4459, 4510, 4514, 4583, 4596, 4666, 4757, 4763, 4803, 4813,
4864, 4888, 4909, 4937, 4959, 5018, 5109, 5137, 5201, 5289, 5298, 5304, 5396,
5400, 5471, 5481, 5494, 5528, 5613, 5669, 5677, 5698, 5703, 5736, 5957, 6012,
6064, 6116, 6128, 6222, 6229, 6345, 6357, 6505, 6645, 6655, 6869, 6945, 7025,
7183, 7266, 7294, 7355, 7404, 7442, 7569, 7634, 7717, 7734, 7747, 8074, 8106,
8140, 8151, 8219, 8223, 8257, 8315, 8324, 8396, 8400, 8442, 8537, 8640, 8645,
8685, 8698, 8745

)

