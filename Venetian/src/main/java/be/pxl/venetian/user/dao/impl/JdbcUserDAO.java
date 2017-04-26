package be.pxl.venetian.user.dao.impl;


import be.pxl.venetian.user.dao.UserDAO;
import be.pxl.venetian.user.model.User;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.RowMapper;

import javax.sql.DataSource;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.List;

public class JdbcUserDAO implements UserDAO {
    private JdbcTemplate jdbcTemplate;

    public JdbcUserDAO(DataSource dataSource) {
        jdbcTemplate = new JdbcTemplate(dataSource);
    }

    public List<User> getAll() {
        String sql="SELECT * FROM users";

        List<User> listUsers = jdbcTemplate.query(sql, new RowMapper<User>() {
            @Override
            public User mapRow(ResultSet resultSet, int i) throws SQLException {
                User user = new User();
                user.setUsername(resultSet.getString(1));
                user.setPassword(resultSet.getString(2));
                return user;
            }
        });

        return listUsers;
    }
}
