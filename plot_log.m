%% Loading CSV
clear;
filename = 'log0.txt';
data = importdata(filename,',',1);
assignin('base', 'time', data.data(:,1));
for i = 2:length(data.colheaders)
    assignin('base', strtrim(data.colheaders{i}), data.data(:,i));
end

%% Mapping Time
minutes = 0;
time_s = zeros(size(time));
for t=1:length(time)
    if t > 1 && abs(time(t)-time(t-1)) > 50
        minutes = minutes + 60;
    end
    time_s(t) = time(t)+minutes;
end
time_l = (1:length(time_s))'*(time_s(end) - time_s(1))/length(time_s) + time_s(1);
dt = mean(diff(time_l));

%% Process angles
phi = -deg2rad(wrapTo180(phi));
theta = -deg2rad(wrapTo180(theta));
psi = deg2rad(wrapTo180(psi));
phi_ref = deg2rad(phi_ref);
theta_ref = deg2rad(theta_ref);
psi_ref = deg2rad(psi_ref);

%% Constants
m=0.4;
Ix = 0.0010;
Iy = 0.0012;
Iz = 0.0015;
tiltKp = 5;
yawKp = 2;

%% Compute Quaternion loop
angular_rate_ref = zeros(3,length(time));
for t=1:length(time);
    q_est = angle2quat(psi(t), theta(t), phi(t));
    q_ref = angle2quat(psi_ref(t), theta_ref(t), phi_ref(t));
    q_err = quatmultiply(q_ref,quatconj(q_est));
    angle = 2*acos(q_err(1));
    axis = q_err(2:4)'/sqrt(1-q_err(1)^2);
    angular_rate_ref(:,t) = angle*axis.*[tiltKp;tiltKp;yawKp];
end

%% Display Roll loop
figure,
ax(1) = subplot(311); plot(time_l, rad2deg(phi_ref), 'r', time_l, rad2deg(phi), 'b'); title('Phi');
ax(2) = subplot(312); plot(time_l, rad2deg(p_ref), 'r', time_l, rad2deg(angular_rate_ref(1,:)'), '--r', time_l, rad2deg(p), 'b', time_l, rad2deg(1/Ix*cumsum(torque_x)*dt), '--k'); title('p')
ax(3) = subplot(313); plot(time_l, torque_x); title('Motor torque x')
linkaxes(ax, 'x')

%% Display Pitch loop
figure,
ax(1) = subplot(311); plot(time_l, rad2deg(theta_ref), 'r', time_l, rad2deg(theta), 'b'); title('Theta');
ax(2) = subplot(312); plot(time_l, rad2deg(q), 'b', time_l, rad2deg(1/Iy*cumsum(torque_y)*dt), '--k'); title('q')
ax(3) = subplot(313); plot(time_l, torque_y); title('Motor torque y')
linkaxes(ax, 'x')
%% Display Yaw loop
figure,
ax(1) = subplot(311); plot(time_l, rad2deg(psi_ref), 'r', time_l, rad2deg(psi), 'b'); title('Psi');
ax(2) = subplot(312); plot(time_l, rad2deg(r), 'b', time_l, rad2deg(1/Iz*cumsum(torque_z)*dt), '--k'); title('r')
ax(3) = subplot(313); plot(time_l, torque_z); title('Motor torque z')
linkaxes(ax, 'x')
%% Display Vz loop
figure,
ax(1) = subplot(211); plot(time_l, vz_ref, 'r', time_l, vz, 'b', time_l, cumsum(thrust_Z/m-9.81)*dt, '--k'); title('vz')
ax(2) = subplot(212); plot(time_l, thrust_Z); title('Motor thrust z')
linkaxes(ax, 'x')