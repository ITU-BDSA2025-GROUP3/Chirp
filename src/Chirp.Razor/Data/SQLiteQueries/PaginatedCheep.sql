/* Queries cheeps ordered by date in ascending order.
   @author, @limit and @offset are to be parameterised in C# for the actual value.
   @author will filter for a specific user or any user if wildcard string is used.
   @limit for the maximum number of cheeps allowed on a page.
   @offset for the page number as a multiple of @limit
*/
SELECT
    u.username AS author,
    m.text AS message,
    m.pub_date AS timestamp
FROM
    user u, message m
WHERE
    u.user_id = m.author_id
    AND
    u.username LIKE @username
ORDER BY timestamp ASC LIMIT @limit OFFSET @offset