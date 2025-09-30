SELECT u.username, m.text, m.pub_date
FROM message m
JOIN user u
ON m.author_id = u.user_id
WHERE u.username = @username
ORDER BY m.pub_date DESC LIMIT @pageSize OFFSET @offset;