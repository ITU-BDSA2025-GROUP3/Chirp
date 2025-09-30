SELECT COUNT(*) AS total_pages
FROM message m
JOIN user u
ON u.user_id = m.author_id
WHERE u.username = @author;